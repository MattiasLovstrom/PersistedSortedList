using System;

namespace PersistedSortedList.Tests
{
    public interface IIndexReader<T> :IDisposable where T : IComparable
    {
        Node<T> NewNode();
        Node<T> Get(int reference);
        void Update(Node<T> root);
        void Commit();
        void RollBack();
    }
}