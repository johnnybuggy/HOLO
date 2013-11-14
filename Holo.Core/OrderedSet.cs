using System;
using System.Collections;
using System.Collections.Generic;

namespace Holo.Core
{
    public class OrderedSet<T> : IList<T>
    {
        private readonly HashSet<T> Set;
        private readonly List<T> List;

        public OrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public OrderedSet(IEqualityComparer<T> comparer)
        {
            Set = new HashSet<T>(comparer);
            List = new List<T>();
        }

        public int Count
        {
            get
            {
                return List.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            lock (this)
            {
                if (Set.Contains(item))
                {
                    return false;
                }

                List.Add(item);

                return true;
            }
        }

        public void Clear()
        {
            lock (this)
            {
                List.Clear();
                Set.Clear();
            }
        }

        public bool Remove(T item)
        {
            lock (this)
            {
                List.Remove(item);
                Set.Remove(item);

                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return Set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (this)
            {
                if (Set.Contains(item))
                {
                    throw new InvalidOperationException("Item already exists.");
                }

                List.Insert(index, item);
                Set.Add(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {
                Set.Remove(List[index]);

                List.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                return List[index];
            }
            set
            {
                lock (this)
                {
                    Set.Remove(List[index]);
                    Set.Add(value);
                    List[index] = value;
                }
            }
        }
    }
}