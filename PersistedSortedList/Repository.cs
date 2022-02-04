using System;
using System.Runtime.Caching;
using System.Text.Json;

namespace PersistedSortedList
{
    public class Repository<T> : IRepository<T>
    {
        private readonly IFileAdapter _fileAdapter;
        private int _last;
        private readonly MemoryCache _cache;

        public Repository(
            IFileAdapter fileAdapter,
            MemoryCache cache)
        {
            _cache = cache;
            _fileAdapter = fileAdapter;
            _last = 1;

        }
        public int Add(T value)
        {
            var startPosition = _last;
            _last = (int)_fileAdapter.Write(_last, JsonSerializer.SerializeToUtf8Bytes(value));
            _cache.Add(startPosition.ToString(), value, new CacheItemPolicy {SlidingExpiration = TimeSpan.FromHours(10)});
            return startPosition;
        }

        public T Get(int position)
        {
            if (_cache.Get(position.ToString()) is T value)
            {
                return value;
            }

            value = JsonSerializer.Deserialize<T>(_fileAdapter.ReadLine(position));
            _cache.Add(position.ToString(), value, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(10) });
            return value;
        }
    }
}