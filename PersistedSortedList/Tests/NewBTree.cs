using System;
using System.Runtime.Caching;

namespace PersistedSortedList.Tests
{
    public class NewBTree<T> where T : IComparable
    {
        private NewNode<T> _root;
        private readonly NewIndexReader<T> _indexReader;
        private readonly IRepository<T> _repository;
        public int Length;
        private int MaxItems;

        public NewBTree(int maxItems)
        {
            MaxItems = maxItems;
            _repository = new Repository<T>(new FileAdapter("test"), MemoryCache.Default);
            _indexReader = new NewIndexReader<T>(_repository);
        }

        public NewBTree(int maxItems, NewIndexReader<T> indexReader, IRepository<T> repository)
        {
            MaxItems = maxItems;
            _indexReader = indexReader;
            _repository = repository;
        }

        public T Get(int key)
        {
            return _root.Get(key);
        }

        public int ReplaceOrInsert(int item)
        {
            if (item == default)
            {
                throw new ArgumentException("Can't be zero", nameof(item));
            }

            if (_root == null)
            {
                _root = _indexReader.NewNode();
                _root.Items.Add(item);
                Length++;
                return default;
            }

            _root = _root.MutableFor(_indexReader);
            if (_root.Items.Count >= MaxItems)
            {
                var (item2, second) = _root.Split(MaxItems / 2);
                var oldRoot = _root;
                _root = _indexReader.NewNode();
                _root.Items.Add(item2);
                _root.Children.Add(oldRoot);
                _root.Children.Add(second);
            }
            var result = _root.Insert(item, MaxItems);
            if (result == default)
            {
                Length++;
            }
            return result;
        }

        public void Print(NewNode<T> current, int level = 0)
        {
            Console.Out.WriteLine(current);
            foreach (var child in current.Children)
            {
                Print(child, level++);
            }
        }
    }
}
