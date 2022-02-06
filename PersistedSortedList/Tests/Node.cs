using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistedSortedList.Tests
{
    public class Node<T> where T : class
    {
        //internal Items<T> Items { get; set; }
        internal List<T> Items { get; set; }
        internal List<Node<T>> Children { get; set; }
        internal CopyOnWriteContext<T> Cow { get; set; }
        internal Comparer<T> Comparer { get; set; }

        public Node(Comparer<T> comparer)
        {
            Comparer = comparer;
            Items = new List<T>();
            Children = new List<Node<T>>();
        }

        public (int, bool) Find(IEnumerable<T> items, T item)
        {
            int index = Items.BinarySearch(0, Items.Count, item, Comparer);

            bool found = index >= 0;

            if (!found)
            {
                index = ~index;
            }

            return index > 0 && !Less(Items[index - 1], item) ? (index - 1, true) : (index, found);
        }

        public T Insert(T item, int maxItems)
        {
            (int i, bool found) = Find(Items, item);
            if (found)
            {
                T n = Items[i];
                Items[i] = item;
                return n;
            }
            if (Children.Count == 0)
            {
                Items.Insert(i, item);
                return null;
            }
            if (MaybeSplitChild(i, maxItems))
            {
                T inTree = Items[i];
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
                    T n = Items[i];
                    Items[i] = item;
                    return n;
                }
            }
            return MutableChild(i).Insert(item, maxItems);
        }

        public T Get(T key)
        {
            var (i, found) = Find(Items, key);
            if (found)
            {
                return Items[i];
            }

            if (Children.Count > 0)
            {
                return Children[i].Get(key);
            }

            return null;
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

        public Node<T> MutableChild(int i)
        {
            var c = Children[i].MutableFor(Cow);
            Children[i] = c;
            return c;
        }

        public Node<T> MutableFor(CopyOnWriteContext<T> cow)
        {
            if (ReferenceEquals(Cow, cow))
            {
                return this;
            }

            Node<T> node = Cow.NewNode();

            node.Items.AddRange(Items);
            node.Children.AddRange(Children);

            return node;
        }

        public (T item, Node<T> node) Split(int i)
        {
            var item = Items[i];
            var next = Cow.NewNode();
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
            return Comparer.Compare(x, y) == -1;
        }

        public override string ToString()
        {
            var message = new StringBuilder("[");
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
