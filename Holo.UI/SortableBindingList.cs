using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Holo.UI
{
    /// <summary>
    /// by Tim Van Wassenhove 
    /// http://www.timvw.be/2008/08/02/presenting-the-sortablebindinglistt-take-two/
    /// </summary>
    public class SortableBindingList<T> : BindingList<T>
    {
        private readonly Dictionary<Type, PropertyComparer<T>> Comparers;
        private bool IsSorted;
        private ListSortDirection ListSortDirection;
        private PropertyDescriptor PropertyDescriptor;

        public SortableBindingList()
            : base(new List<T>())
        {
            Comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        public SortableBindingList(IEnumerable<T> enumeration)
            : base(new List<T>(enumeration))
        {
            Comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        protected override bool IsSortedCore
        {
            get
            {
                return IsSorted;
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return PropertyDescriptor;
            }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return ListSortDirection;
            }
        }

        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> ItemsList = (List<T>)this.Items;

            Type PropertyType = property.PropertyType;
            PropertyComparer<T> Comparer;
            if (!Comparers.TryGetValue(PropertyType, out Comparer))
            {
                Comparer = new PropertyComparer<T>(property, direction);
                Comparers.Add(PropertyType, Comparer);
            }

            Comparer.SetPropertyAndDirection(property, direction);
            ItemsList.Sort(Comparer);

            PropertyDescriptor = property;
            ListSortDirection = direction;
            IsSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            IsSorted = false;
            PropertyDescriptor = base.SortPropertyCore;
            ListSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            for (int i = 0; i < Count; ++i)
            {
                T Element = this[i];
                if (property.GetValue(Element).Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}