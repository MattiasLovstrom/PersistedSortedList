namespace PersistedSortedList
{
    public interface IIndexReader
    {
        Node Create(int parent);
        Node Get(int reference);
        void Update(Node node);
    }
}