using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Holo.UI
{
    /// <summary>
    /// by Tim Van Wassenhove 
    /// http://www.timvw.be/2008/08/02/presenting-the-sortablebindinglistt-take-two/
    /// </summary>
    public class PropertyComparer<T> : IComparer<T>
    {
        private readonly IComparer Comparer;
        private PropertyDescriptor PropertyDescriptor;
        private int Reverse;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            PropertyDescriptor = property;
            Type ComparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            Comparer = (IComparer)ComparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            SetListSortDirection(direction);
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return Reverse * Comparer.Compare(PropertyDescriptor.GetValue(x), PropertyDescriptor.GetValue(y));
        }

        #endregion

        private void SetPropertyDescriptor(PropertyDescriptor descriptor)
        {
            PropertyDescriptor = descriptor;
        }

        private void SetListSortDirection(ListSortDirection direction)
        {
            Reverse = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
        {
            SetPropertyDescriptor(descriptor);
            SetListSortDirection(direction);
        }
    }
}