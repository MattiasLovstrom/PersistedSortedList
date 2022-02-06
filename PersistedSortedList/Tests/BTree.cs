using System;

namespace PersistedSortedList.Tests
{
    public class BTree
    {
        private readonly int _degree;
        private Node _root;
        private readonly IndexReader _indexReader;
        public int Length;

        public BTree(int degree)
            : this(degree, new IndexReader())
        { }

        public BTree(int degree, IndexReader indexReader)
        {
            if (degree <= 1)
            {
                Environment.FailFast("bad degree");
            }
            _degree = degree;
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
                Environment.FailFast("null item being added to BTree");
            }

            if (_root == null)
            {
                _root = _indexReader.NewNode();
                _root.Items.Add(item);
                Length++;
                return default;
            }

            _root = _root.MutableFor(_indexReader);
            if (_root.Items.Count >= MaxItems())
            {
                var (item2, second) = _root.Split(MaxItems() / 2);
                var oldRoot = _root;
                _root = _indexReader.NewNode();
                _root.Items.Add(item2);
                _root.Children.Add(oldRoot);
                _root.Children.Add(second);
            }
            var result = _root.Insert(item, MaxItems());
            if (result == default)
            {
                Length++;
            }
            return result;
        }

        private int MaxItems()
        {
            return _degree * 2 - 1;
        }

        public void Print(Node current, int level = 0)
        {
            Console.Out.WriteLine(current);
            foreach (var child in current.Children)
            {
                Print(child, level++);
            }
        }
    }
}
