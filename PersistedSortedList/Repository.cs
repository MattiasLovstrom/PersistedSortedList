using System.Text.Json;
using PersistedSortedList;

namespace SortedFileList
{
    public class Repository<T> : IRepository<T>
    {
        private IFileAdapter _fileAdapter;
        private int _last;

        public Repository(string name)
        {
            _fileAdapter = new FileAdapter(name + ".db");
            _last = 1;

        }
        public int Add(T value)
        {
            var startPosition = _last;
            _last = (int)_fileAdapter.Write(_last, JsonSerializer.SerializeToUtf8Bytes(value));

            return startPosition;
        }

        public T Get(int position)
        {
            return JsonSerializer.Deserialize<T>(_fileAdapter.ReadLine(position));
        }

    }
}