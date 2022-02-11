using System;
using System.Runtime.Caching;
using PersistedSortedList.Tests;

namespace PersistedSortedList
{
    public class PersistedSortedList<T> : IDisposable where T : IComparable
    {
        private readonly IIndex<T> _index;
        private readonly IRepository<T> _repository;
        private FileAdapter _repositoryFile;
        private FileAdapter _indexFile;

        public PersistedSortedList(string name)
        {
            var cache = MemoryCache.Default;

            _repositoryFile = new FileAdapter(name + ".db");
            _repository = new Repository<T>(_repositoryFile, cache);

            _indexFile = new FileAdapter(name + ".index");
            var indexReader = new NewIndexReader<T>(
                _repository);

            _index = new Index<T>(indexReader, _repository);
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
            _index.Add(position);
        }

        public T Get(T prototype)
        {
            return _index.Get(prototype);
        }

        public void Dispose()
        {
            _repositoryFile?.Dispose();
            _indexFile?.Dispose();
        }
    }
}