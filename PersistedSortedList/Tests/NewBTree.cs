using System;

namespace PersistedSortedList.Tests
{
    public class NewBTree<T> where T : IComparable
    {
        private NewNode<T> _root;
        private readonly INewIndexReader<T> _indexReader;
        public int Length;
        
        public NewBTree(INewIndexReader<T> indexReader)
        {
            _indexReader = indexReader;
        }

        public T Get(T prototype)
        {
            return _root.Get(prototype);
        }

        public int Add(int fileReference)
        {
            Console.Out.WriteLine($"Add {fileReference.ToString("X8")} in {_root}");
            if (_root == null)
            {
                _root = _indexReader.NewNode();
                _root.Items.Add(fileReference);
                Console.Out.WriteLine("Create root :" + _root);
                Length++;
                _indexReader.Update(_root);
                return default;
            }

            if (_root.Items.Count >= Constants.BranchingFactor)
            {
                var (item2, second) = _root.Split(Constants.BranchingFactor / 2);
                var oldRoot = _root;
                var oldRootPosition = oldRoot.Position;
                _root = _indexReader.NewNode();
                oldRoot.Position = _root.Position;
                _root.Position = oldRootPosition;
                _root.Items.Add(item2);
                _root.Children.Add(oldRootPosition);
                _root.Children.Add(second.Position);
                _indexReader.Update(_root);
                _indexReader.Update(oldRoot);
                Console.Out.WriteLine("New root :" + _root);
            }
            var result = _root.Insert(fileReference, Constants.BranchingFactor);
            if (result == default)
            {
                Length++;
            }
            //_indexReader.Update(_root);
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
