using System;

namespace PersistedSortedList.Tests
{
    public class NewBTree
    {
        private NewNode _root;
        private readonly NewIndexReader _indexReader;
        public int Length;
        private int MaxItems;

        public NewBTree(int degree)
            : this(degree, new NewIndexReader())
        { }

        public NewBTree(int maxItems, NewIndexReader indexReader)
        {
            MaxItems = maxItems;
            _indexReader = indexReader;
        }

        public int Get(int key)
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

        public void Print(NewNode current, int level = 0)
        {
            Console.Out.WriteLine(current);
            foreach (var child in current.Children)
            {
                Print(child, level++);
            }
        }
    }
}
