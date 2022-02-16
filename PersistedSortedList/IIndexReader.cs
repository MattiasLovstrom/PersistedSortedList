using System;

namespace PersistedSortedList.Tests
{
    public interface IIndexReader<T> where T : IComparable
    {
        Node<T> NewNode();
        Node<T> Get(int reference);
        void Update(Node<T> root);
    }
}