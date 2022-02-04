using System;
using System.Runtime.Caching;

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
            _indexFile = new FileAdapter(name + ".index");
            var indexReader = new IndexReader(
                _indexFile,
                cache);

            _repositoryFile = new FileAdapter(name + ".db");

            _repository = new Repository<T>(_repositoryFile, cache);
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
            _index.Add(position, value);
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