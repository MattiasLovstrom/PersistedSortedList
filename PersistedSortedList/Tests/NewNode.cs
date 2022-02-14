using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PersistedSortedList.Tests
{
    public class NewNode<T> where T : IComparable
    {
        public List<int> Items { get; set; }
        public readonly List<int> Children;
        private readonly INewIndexReader<T> _indexReader;
        private readonly IRepository<T> _repository;
        public int Position { get; set; }

        public NewNode(
            INewIndexReader<T> indexReader,
            IRepository<T> repository)
        {
            _indexReader = indexReader;
            _repository = repository;
            Items = new List<int>();
            Children = new List<int>();
        }

        public bool TryGetIndexOfReference(int fileReference, out int index)
        {
            for (index = 0; index < Items.Count; index++)
            {
                if (Items[index] == fileReference)
                {
                    return true;
                }
            }

            var searchFor = _repository.Get(fileReference);
            for (index = 0; index < Items.Count; index++)
            {
                var item = _repository.Get(Items[index]);
                if (searchFor.CompareTo(item) < 0)
                {
                    return false;
                }
            }
            return false;
        }

        public bool TryGetValue(T prototype, out int index)
        {
            for (index = 0; index < Items.Count; index++)
            {
                var item = _repository.Get(Items[index]);
                if (prototype.CompareTo(item) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public int Insert(int fileReference, int branchingFactor)
        {
            if (TryGetIndexOfReference(fileReference, out var i))
            {
                var n = Items[i];
                Items[i] = fileReference;
                Console.Out.WriteLine($"Insert {fileReference.ToString("X8")} in {this}");
                return n;
            }
            if (Children.Count == 0)
            {
                Items.Insert(i, fileReference);
                Console.Out.WriteLine($"Insert {fileReference.ToString("X8")} in leaf {this}");
                return default;
            }

            var child = _indexReader.Get(Children[i]);
            if (child.Items.Count >= branchingFactor)
            {
                SplitChild(i, branchingFactor);
                var inTree = Items[i];
                var item = _repository.Get(fileReference);
                var inTreeItem = _repository.Get(inTree);

                if (item.CompareTo(inTreeItem) < 0)
                {
                    // no change, we want first split node
                }
                else if (inTreeItem.CompareTo(item) < 0)
                {
                    i++; // we want second split node
                }
                else
                {
                    var n = Items[i];
                    Items[i] = fileReference;
                    Console.Out.WriteLine($"Insert {fileReference.ToString("X8")} in {this}");
                    return n;
                }
            }
            return _indexReader.Get(Children[i]).Insert(fileReference, branchingFactor);
        }

        public T Get(T prototype)
        {
            if (TryGetValue(prototype, out var i))
            {
                return _repository.Get(Items[i]);
            }

            if (Children.Count > 0)
            {
                var child = _indexReader.Get(Children[i]);
                return child.Get(prototype);
            }

            return default;
        }
        public void SplitChild(int i, int maxItems)
        {
            var first = _indexReader.Get(Children[i]);
            Console.Out.WriteLine($"Split child {i} {first}");
            var (item, second) = first.Split(maxItems / 2);
            Items.Insert(i, item);
            Children.Insert(i + 1, second.Position);
            Console.Out.WriteLine($"Split to {this} and {second}");
        }

        public (int item, NewNode<T> node) Split(int i)
        {
            var item = Items[i];
            var next = _indexReader.NewNode();
            next.Items.AddRange(Items.GetRange(i + 1, Items.Count - (i + 1)));
            Items.RemoveRange(i, Items.Count - i);
            if (Children.Count > 0)
            {
                next.Children.AddRange(Children.GetRange(i + 1, Children.Count - (i + 1)));
                var count = Children.Count - (i + 1);
                if (count > 0)
                {
                    Children.RemoveRange(i + 1, count);
                }
            }
            return (item, next);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder("");
            if (Items.Any())
            {
                message
                    .Append("[")
                    .Append(Items.Select(i => _repository.Get(i).ToString()).Aggregate((c, n) => c + "," + n))
                    .Append("]");
            }

            if (Children.Any())
            {
                message.Append("=>");

                message
                    .Append("[")
                    .Append(Children.Select(c => _indexReader.Get(c).ToString()).Aggregate((c, n) => c + "," + n))
                    .Append("]");
            }

            return message.ToString();
        }
    }
}
