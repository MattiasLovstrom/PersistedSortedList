using SortedFileList;
using System;

namespace PersistedSortedList
{
    public class BTree<T> where T : IComparable
    {
        private readonly IIndexReader _nodeReader;
        private readonly IRepository<T> _repository;
        private readonly Node _root;

        public BTree(IIndexReader nodeReader,
            IRepository<T> repository)
        {
            _nodeReader = nodeReader;
            _repository = repository;
            _root = nodeReader.Create();
        }

        public Node Add(int position, T value)
        {
            return Add(position, value, _root);
        }

        public Node Add(int position, T value, Node current)
        {
            int i;
            for (i = 0; i < current.Values.Length; i++)
            {
                if (current.Values[i] == 0)
                {
                    current.Values[i] = position;
                    _nodeReader.Update(current);
                    return current;
                }

                var v = _repository.Get(current.Values[i]);
                if (value.CompareTo(v) <= 0) break;
            }

            if (current.References[i] == 0)
            {
                var newNode = _nodeReader.Create();
                current.References[i] = newNode.Position;
                _nodeReader.Update(current);
                return Add(position, value, newNode);
            }

            return Add(position, value, _nodeReader.Get(current.References[i]));
        }

        public T Get(T prototype)
        {
            return Get(prototype, _root);
        }

        public T Get(T value, Node current)
        {
            int i;
            for (i = 0; i < current.Values.Length; i++)
            {
                var v = _repository.Get(current.Values[i]);
                if (value.CompareTo(v) == 0) return v;
                if (value.CompareTo(v) <= 0) break;
            }

            return Get(value, _nodeReader.Get(current.References[i]));
        }
    }
}