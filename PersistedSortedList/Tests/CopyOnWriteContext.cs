namespace PersistedSortedList.Tests
{
    public class CopyOnWriteContext<T> where T : class
    {
        public FreeList<T> FreeList { get; internal set; }

        public Node<T> NewNode()
        {
            Node<T> n = FreeList.NewNode();
            n.Cow = this;
            return n;
        }

        // Frees a node within a given COW context, if it's owned by that
        // context.  It returns what happened to the node (see freeType const
        // documentation).        
        public FreeType FreeNode(Node<T> n)
        {
            if (ReferenceEquals(n.Cow, this))
            {
                // clear to allow GC
                n.Items.Truncate(0);
                n.Children.Truncate(0);
                n.Cow = null;
                return FreeList.FreeNode(n) ? FreeType.ftStored : FreeType.ftFreeListFull;
            }
            else
            {
                return FreeType.ftNotOwned;
            }
        }
    }
}