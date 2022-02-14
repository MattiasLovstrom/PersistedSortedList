using System;
using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    public class NewIndexReader<T> : INewIndexReader<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private readonly List<NewNode<T>> _list;

        public NewIndexReader(IRepository<T> repository)
        {
            _repository = repository;
            _list = new List<NewNode<T>>();
        }

        public NewNode<T> NewNode()
        {
            var newNode = new NewNode<T>(this, _repository);
            newNode.Position = _list.Count;
            _list.Add(newNode);
            
            return newNode;
        }

        public NewNode<T> Get(int reference)
        {
            return _list[reference];
        }
    }
}