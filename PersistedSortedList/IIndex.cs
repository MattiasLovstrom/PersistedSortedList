using System;
using PersistedSortedList.Tests;

namespace PersistedSortedList
{
    public interface IIndex<T>
    {
        void Add(long fileReference);
        T Get(T prototype);
    }

    public class Index<T> : IIndex<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private BTree<T> _tree;

        public Index(IIndexReader<T> indexReader, IRepository<T> repository)
        {
            _repository = repository;
            _tree = new BTree<T>(indexReader);
        }

        public void Add(long fileReference)
        {
            _tree.Add((int)fileReference);
        }

        public T Get(T prototype)
        {
            return _tree.Get(prototype);
        }
    }
}