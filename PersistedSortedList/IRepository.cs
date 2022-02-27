using System;

namespace PersistedSortedList
{
    public interface IRepository<T> : IDisposable
    {
        int Add(T value);
        T Get(int position);
    }
}