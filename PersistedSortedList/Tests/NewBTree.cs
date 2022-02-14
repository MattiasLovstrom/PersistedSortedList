using System;
using System.Runtime.Caching;

namespace PersistedSortedList.Tests
{
    public class NewBTree<T> where T : IComparable
    {
        private NewNode<T> _root;
        private readonly INewIndexReader<T> _indexReader;
        private readonly IRepository<T> _repository;
        public int Length;
        private int BranchingFactor;

        public NewBTree(int branchingFactor)
        {
            BranchingFactor = branchingFactor;
            _repository = new Repository<T>(new FileAdapter("test"), MemoryCache.Default);
            _indexReader = new NewIndexReader<T>(_repository);
        }

        public NewBTree(
            int branchingFactor, 
            INewIndexReader<T> indexReader, 
            IRepository<T> repository)
        {
            BranchingFactor = branchingFactor;
            _indexReader = indexReader;
            _repository = repository;
        }

        public T Get(T prototype)
        {
            return _root.Get(prototype);
        }

        public int ReplaceOrInsert(int fileReference)
        {
            if (fileReference == default)
            {
                throw new ArgumentException("Can't be zero", nameof(fileReference));
            }

            Console.Out.WriteLine($"ReplaceOrInsert {fileReference.ToString("X8")} in {_root}");
            if (_root == null)
            {
                _root = _indexReader.NewNode();
                _root.Items.Add(fileReference);
                Console.Out.WriteLine("Create root :" + _root);
                Length++;
                return default;
            }

            //_root = _root.MutableFor(_indexReader);
            if (_root.Items.Count >= BranchingFactor)
            {
                var (item2, second) = _root.Split(BranchingFactor / 2);
                var oldRoot = _root;
                _root = _indexReader.NewNode();
                _root.Items.Add(item2);
                _root.Children.Add(oldRoot.Position);
                _root.Children.Add(second.Position);
                Console.Out.WriteLine("New root :" + _root);
            }
            var result = _root.Insert(fileReference, BranchingFactor);
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
                var node = _indexReader.Get(child);
                Print(node, level++);
            }
        }
    }
}
