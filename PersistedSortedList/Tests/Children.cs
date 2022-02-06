using System.Collections;
using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    internal class Children<T> : IEnumerable<Node<T>> where T : class
    {
        private readonly List<Node<T>> _children = new List<Node<T>>();

        public int Length => _children.Count;

        public Node<T> this[int i]
        {
            get => _children[i];
            set => _children[i] = value;
        }

        public void Append(Node<T> node)
        {
            _children.Add(node);
        }

        public void Append(IEnumerable<Node<T>> range)
        {
            _children.AddRange(range);
        }

        public void InsertAt(int index, Node<T> item)
        {
            _children.Insert(index, item);
        }
        public List<Node<T>> GetRange(int index, int count)
        {
            return _children.GetRange(index, count);
        }

        public void Truncate(int index)
        {
            int count = _children.Count - index;
            if (count > 0)
            {
                _children.RemoveRange(index, count);
            }
        }

        IEnumerator<Node<T>> IEnumerable<Node<T>>.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}