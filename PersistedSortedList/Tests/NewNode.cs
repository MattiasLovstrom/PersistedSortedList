using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistedSortedList.Tests
{
    public class NewNode<T> where T : IComparable
    {
        public List<int> Items { get; set; }
        public readonly List<NewNode<T>> Children;
        private readonly NewIndexReader<T> _indexReader;
        private readonly IRepository<T> _repository;

        public NewNode(
            NewIndexReader<T> indexReader,
            IRepository<T> repository)
        {
            _indexReader = indexReader;
            _repository = repository;
            Items = new List<int>();
            Children = new List<NewNode<T>>();
        }

        public bool TryGetReference(int fileReference, out int index)
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
                if (Less(searchFor, item))
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

        public int Insert(int fileReference, int maxItems)
        {
            if (TryGetReference(fileReference, out var i))
            {
                var n = Items[i];
                Items[i] = fileReference;
                return n;
            }
            if (Children.Count == 0)
            {
                Items.Insert(i, fileReference);
                return default;
            }
            if (MaybeSplitChild(i, maxItems))
            {
                var inTree = Items[i];
                var item = _repository.Get(fileReference);
                var inTreeItem = _repository.Get(inTree);

                if (Less(item, inTreeItem))
                {
                    // no change, we want first split node
                }
                else if (Less(inTreeItem, item))
                {
                    i++; // we want second split node
                }
                else
                {
                    var n = Items[i];
                    Items[i] = fileReference;
                    return n;
                }
            }
            return MutableChild(i).Insert(fileReference, maxItems);
        }

        public T Get(T prototype)
        {
            if (TryGetValue(prototype, out var i))
            {
                return _repository.Get(Items[i]);
            }

            if (Children.Count > 0)
            {
                return Children[i].Get(prototype);
            }

            return default;
        }
        public bool MaybeSplitChild(int i, int maxItems)
        {
            if (Children[i].Items.Count < maxItems)
            {
                return false;
            }
            var first = MutableChild(i);
            var (item, second) = first.Split(maxItems / 2);
            Items.Insert(i, item);
            Children.Insert(i + 1, second);
            return true;
        }

        public NewNode<T> MutableChild(int i)
        {
            var c = Children[i].MutableFor(_indexReader);
            Children[i] = c;
            return c;
        }

        public NewNode<T> MutableFor(NewIndexReader<T> cow)
        {
            if (ReferenceEquals(_indexReader, cow))
            {
                return this;
            }

            var node = _indexReader.NewNode();

            node.Items.AddRange(Items);
            node.Children.AddRange(Children);

            return node;
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

        private bool Less(T x, T y)
        {
            return x.CompareTo(y) < 0;
        }

        public override string ToString()
        {
            //[1<2>3]<4>[5<6>7]
            var message = new StringBuilder("");
            if (Items.Any())
            {
                message.Append(Items.Select(i => i.ToString()).Aggregate((c, n) => c + "," + n))
                    .Append("]");
            }

            if (Children.Any())
            {
                message.Append("=>");

                message.Append(Children.Select(c => c.ToString()).Aggregate((c, n) => c + "," + n))
                    .Append("]");
            }

            return message.ToString();
        }
    }
}
