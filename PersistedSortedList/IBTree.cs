namespace PersistedSortedList.Tests
{
    public interface IBTree<T>
    {
        T Get(T prototype);
        int Add(int fileReference);
    }
}