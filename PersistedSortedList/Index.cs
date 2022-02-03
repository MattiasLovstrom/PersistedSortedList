using System;
using SortedFileList;

namespace PersistedSortedList
{
    public class Index<T> : IIndex<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private IndexReader _indexReader;
        private Node _root;

        public Index(
            string name,
            IRepository<T> repository)
        {
            _repository = repository;
            _indexReader = new IndexReader(name);
            _root = _indexReader.Create();
        }

        public void Add(long fileReference, T value)
        {
           Add((int)fileReference, value, _root);
        }

        public Node Add(int position, T value, Node current)
        {
            int i;
            for (i = 0; i < current.Values.Length; i++)
            {
                if (current.Values[i] == 0)
                {
                    current.Values[i] = position;
                    _indexReader.Update(current);
                    return current;
                }

                var v = _repository.Get(current.Values[i]);
                if (value.CompareTo(v) <= 0) break;
            }

            if (current.References[i] == 0)
            {
                var newNode = _indexReader.Create();
                current.References[i] = newNode.Position;
                _indexReader.Update(current);
                return Add(position, value, newNode);
            }

            return Add(position, value, _indexReader.Get(current.References[i]));
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

            return Get(value, _indexReader.Get(current.References[i]));
        }
    }
}