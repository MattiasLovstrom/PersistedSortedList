using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistedSortedList.Tests
{
    public class NewNode<T> where T : IComparable
    {
        public readonly List<int> Items;
        public readonly List<NewNode<T>> Children;
        private readonly NewIndexReader<T> _indexReader;

        public NewNode(
            NewIndexReader<T> indexReader,
            IRepository<T> repository)
        {
            _indexReader = indexReader;
            Items = new List<int>();
            Children = new List<NewNode<T>>();
        }

        public bool TryGetValue(int item, out int index)
        {
            index = Items.BinarySearch(0, Items.Count, item, Comparer<int>.Default);

            var found = index >= 0;

            if (!found)
            {
                index = ~index;
            }

            if (index <= 0 || Less(Items[index - 1], item))
            {
                return found;
            }

            index--;
            return true;
        }

        public int Insert(int item, int maxItems)
        {
            if (TryGetValue(item, out var i))
            {
                var n = Items[i];
                Items[i] = item;
                return n;
            }
            if (Children.Count == 0)
            {
                Items.Insert(i, item);
                return default;
            }
            if (MaybeSplitChild(i, maxItems))
            {
                var inTree = Items[i];
                if (Less(item, inTree))
                {
                    // no change, we want first split node
                }
                else if (Less(inTree, item))
                {
                    i++; // we want second split node
                }
                else
                {
                    var n = Items[i];
                    Items[i] = item;
                    return n;
                }
            }
            return MutableChild(i).Insert(item, maxItems);
        }

        public int Get(int key)
        {
            if (TryGetValue(key, out var i))
            {
                return Items[i];
            }

            if (Children.Count > 0)
            {
                return Children[i].Get(key);
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

        private bool Less(int x, int y)
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
