using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistedSortedList.Tests
{
    public class Node<T> where T : class
    {
        internal Items<T> Items { get; set; }
        internal Children<T> Children { get; set; }
        internal CopyOnWriteContext<T> Cow { get; set; }
        internal Comparer<T> Comparer { get; set; }

        public Node(Comparer<T> comparer)
        {
            Comparer = comparer;
            Items = new Items<T>(comparer);
            Children = new Children<T>();
        }

        public T Insert(T item, int maxItems)
        {
            (int i, bool found) = Items.Find(item);
            if (found)
            {
                T n = Items[i];
                Items[i] = item;
                return n;
            }
            if (Children.Length == 0)
            {
                Items.InsertAt(i, item);
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
            (int i, bool found) = Items.Find(key);
            if (found)
            {
                return Items[i];
            }
            else if (Children.Length > 0)
            {
                return Children[i].Get(key);
            }
            return null;
        }
        public bool MaybeSplitChild(int i, int maxItems)
        {
            if (Children[i].Items.Length < maxItems)
            {
                return false;
            }
            Node<T> first = MutableChild(i);
            (T item, Node<T> second) = first.Split(maxItems / 2);
            Items.InsertAt(i, item);
            Children.InsertAt(i + 1, second);
            return true;
        }

        public Node<T> MutableChild(int i)
        {
            Node<T> c = Children[i].MutableFor(Cow);
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

            node.Items.Append(Items);
            node.Children.Append(Children);

            return node;
        }

        public (T item, Node<T> node) Split(int i)
        {
            T item = Items[i];
            Node<T> next = Cow.NewNode();
            next.Items.Append(Items.GetRange(i + 1, Items.Length - (i + 1)));
            Items.Truncate(i);
            if (Children.Length > 0)
            {
                next.Children.Append(Children.GetRange(i + 1, Children.Length - (i + 1)));
                Children.Truncate(i + 1);
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
