using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    public class IndexReader 
    {
        private const int DefaultFreeListSize = 32;

        private readonly object _lockObject;
        private readonly List<Node> _freelist;

        public IndexReader()
        {
            _lockObject = new object();
            _freelist = new List<Node>(DefaultFreeListSize);
        }
        
        public Node NewNode()
        {
            lock (_lockObject)
            {
                int index = _freelist.Count - 1;

                if (index < 0)
                {
                    return new Node(this);
                }

                var n = _freelist[index];

                _freelist[index] = null;
                _freelist.RemoveAt(index);

                return n;
            }
        }
    }
}