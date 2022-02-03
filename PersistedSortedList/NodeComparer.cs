using System;
using System.Collections;
using PersistedSortedList;

namespace SortedFileList
{
    public class NodeComparer<T> : IComparer 
    {
        private readonly IRepository<T> _repository;

        public NodeComparer(IRepository<T> repository)
        {
            _repository = repository;
        }

        public int Compare(object? x, object? y)
        {
            TestObject n1;
            if (x is TestObject x1)
            {
                n1 = x1;
            }
            else
            {
                n1 = _repository.Get((int) x) as TestObject;
            }
            
            var n2 = _repository.Get((int)y) as TestObject;

            return n1.Value.CompareTo(n2.Value);
        }
    }
}