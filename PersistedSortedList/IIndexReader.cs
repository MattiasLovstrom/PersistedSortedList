namespace PersistedSortedList
{
    public interface IIndexReader
    {
        Node1 Create(int parent);
        Node1 Get(int reference);
        void Update(Node1 node);
    }
}