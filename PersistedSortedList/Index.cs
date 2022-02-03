using System;
using System.Xml.Linq;
using PersistedSortedList;

namespace SortedFileList
{
    public class Index<T> : IIndex<T> 
    {
        private readonly IRepository<T> _repository;
        private BTree _tree;

        public Index(
            string name,
            IRepository<T> repository)
        {
            _repository = repository;
            var indexReader = new IndexReader(name);
            _tree = new BTree(new NodeComparer<T>(repository), indexReader);
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