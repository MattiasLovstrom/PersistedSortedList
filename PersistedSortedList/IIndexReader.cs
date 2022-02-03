namespace PersistedSortedList
{
    public interface IIndexReader
    {
        Node Create();
        Node Get(int reference);
        void Update(Node node);
    }
}