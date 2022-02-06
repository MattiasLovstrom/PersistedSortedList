using System.Collections;
using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    internal class Items<T> : IEnumerable<T> where T : class
    {
        private readonly List<T> _items = new List<T>();
        private readonly Comparer<T> _comparer;
        public int Length => _items.Count;
        public Items(Comparer<T> comparer)
        {
            _comparer = comparer;
        }

        public (int, bool) Find(T item)
        {
            int index = _items.BinarySearch(0, _items.Count, item, _comparer);

            bool found = index >= 0;

            if (!found)
            {
                index = ~index;
            }

            return index > 0 && !Less(_items[index - 1], item) ? (index - 1, true) : (index, found);
        }

        private bool Less(T x, T y)
        {
            return _comparer.Compare(x, y) == -1;
        }

        public T this[int i]
        {
            get => _items[i];
            set => _items[i] = value;
        }

        public void InsertAt(int index, T item)
        {
            _items.Insert(index, item);
        }

        public void Append(T item)
        {
            _items.Add(item);
        }
        public void Append(IEnumerable<T> items)
        {
            _items.AddRange(items);
        }
        public void Truncate(int index)
        {
            int count = _items.Count - index;
            if (count > 0)
            {
                _items.RemoveRange(index, count);
            }
        }

        public List<T> GetRange(int index, int count)
        {
            return _items.GetRange(index, count);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}