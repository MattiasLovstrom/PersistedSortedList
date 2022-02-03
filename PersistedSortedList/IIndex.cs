namespace PersistedSortedList
{
    public interface IIndex<T>
    {
        void Add(long fileReference, T value);
        T Get(T prototype);
    }
}