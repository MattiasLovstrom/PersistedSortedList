using System;
using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    public class NewIndexReader<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private const int DefaultFreeListSize = 32;

        private readonly object _lockObject;
        private readonly List<NewNode<T>> _freelist;

        public NewIndexReader(IRepository<T> repository)
        {
            _repository = repository;
            _lockObject = new object();
            _freelist = new List<NewNode<T>>(DefaultFreeListSize);
        }
        
        public NewNode<T> NewNode()
        {
            lock (_lockObject)
            {
                var index = _freelist.Count - 1;

                if (index < 0)
                {
                    return new NewNode<T>(this, _repository);
                }

                var n = _freelist[index];

                _freelist[index] = null;
                _freelist.RemoveAt(index);

                return n;
            }
        }
    }
}