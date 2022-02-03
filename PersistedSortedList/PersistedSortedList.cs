using System;
using SortedFileList;

namespace PersistedSortedList
{
    public class PersistedSortedList<T> where T : IComparable
    {
        private readonly IIndex<T> _index;
        private readonly IRepository<T> _repository;


        public PersistedSortedList(string name)
        {
            _repository = new Repository<T>(name);
            _index = new Index<T>(name, _repository);
        }

        public PersistedSortedList(
            IRepository<T> repository, 
            IIndex<T> index)
        {
            _repository = repository;
            _index = index;
        }

        public void Add(T value)
        {
            var position = _repository.Add(value);
            _index.Add(position, value);
        }

        public T Get(T prototype)
        {
            return _index.Get(prototype);
        }
    }
}