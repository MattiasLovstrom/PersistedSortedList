using System;
using SortedFileList;

namespace PersistedSortedList
{
    public class Index<T> : IIndex<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private readonly BTree<T> _tree;

        public Index(
            string name,
            IRepository<T> repository)
        {
            _repository = repository;
            var indexReader = new IndexReader(name);
            _tree = new BTree<T>(indexReader, _repository);
        }

        public void Add(long fileReference, T value)
        {
            _tree.Add((int)fileReference, value);
        }

        public T Get(T prototype)
        {
            return _tree.Get(prototype);
        }
    }
}