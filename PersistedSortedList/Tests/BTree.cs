using System;
using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    public class BTree<T> where T : class
    {
        private int Degree { get; set; }
        private Node<T> Root { get; set; }
        private CopyOnWriteContext<T> Cow { get; set; }
        public int Length { get; private set; }

        public BTree(int degree, Comparer<T> comparer)
            : this(degree, new FreeList<T>(comparer))
        { }

        public BTree(int degree, FreeList<T> f)
        {
            if (degree <= 1)
            {
                Environment.FailFast("bad degree");
            }
            Degree = degree;
            Cow = new CopyOnWriteContext<T> { FreeList = f };
        }
        public T Get(T key)
        {
            return Root?.Get(key);
        }

        public T ReplaceOrInsert(T item)
        {
            if (item == null)
            {
                Environment.FailFast("null item being added to BTree");
            }
            if (Root == null)
            {
                Root = Cow.NewNode();
                Root.Items.Add(item);
                Length++;
                return null;
            }
            else
            {
                Root = Root.MutableFor(Cow);
                if (Root.Items.Count >= MaxItems())
                {
                    var (item2, second) = Root.Split(MaxItems() / 2);
                    var oldRoot = Root;
                    Root = Cow.NewNode();
                    Root.Items.Add(item2);
                    Root.Children.Add(oldRoot);
                    Root.Children.Add(second);
                }
            }
            T result = Root.Insert(item, MaxItems());
            if (result == null)
            {
                Length++;
            }
            return result;
        }

        private int MaxItems()
        {
            return (Degree * 2) - 1;
        }

        public void Print(Node<T> current, int level = 0)
        {
            Console.Out.WriteLine(current);
            foreach (var child in current.Children)
            {
                Print(child, level++);
            }
        }
    }
}
