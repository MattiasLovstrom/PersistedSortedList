using System.Text;

namespace PersistedSortedList
{
    public class Node
    {
        public int Position { get; set; }
        public int[] Values = new int[BTree.BranchingFactor];
        public int[] References = new int[BTree.BranchingFactor + 1];

        public override string ToString()
        {
            return Encoding.UTF8.GetString(BTree.Serialize(this));
        }
    }
}