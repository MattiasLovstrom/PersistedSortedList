using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    public class FreeList<T> where T : class
    {
        private const int DefaultFreeListSize = 32;

        private readonly object _mu;
        private readonly List<Node<T>> _freelist;
        private readonly Comparer<T> _comparer;

        /// <summary>
        /// Creates a new free list with default size.
        /// </summary>
        /// <param name="comparer"></param>
        public FreeList(Comparer<T> comparer)
            : this(DefaultFreeListSize, comparer)
        { }

        /// <summary>
        /// Creates a new free list.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="comparer"></param>
        public FreeList(int size, Comparer<T> comparer)
        {
            _mu = new object();
            _freelist = new List<Node<T>>(size);
            _comparer = comparer;
        }

        internal Node<T> NewNode()
        {
            lock (_mu)
            {
                int index = _freelist.Count - 1;

                if (index < 0)
                {
                    return new Node<T>(_comparer);
                }

                Node<T> n = _freelist[index];

                _freelist[index] = null;
                _freelist.RemoveAt(index);

                return n;
            }
        }

        // Adds the given node to the list, returning true if it was added
        // and false if it was discarded.           
        internal bool FreeNode(Node<T> n)
        {
            bool success = false;

            lock (_mu)
            {
                if (_freelist.Count < _freelist.Capacity)
                {
                    _freelist.Add(n);
                    success = true;
                }
            }

            return success;
        }
    }
}