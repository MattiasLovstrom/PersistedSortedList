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
        private NewBTree<T> _tree;

        public Index(INewIndexReader<T> indexReader, IRepository<T> repository)
        {
            _repository = repository;
            _tree = new NewBTree<T>(indexReader);
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