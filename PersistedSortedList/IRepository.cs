namespace PersistedSortedList
{
    public interface IRepository<T>
    {
        int Add(T value);
        T Get(int position);
    }
}