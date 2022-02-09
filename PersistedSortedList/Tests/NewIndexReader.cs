using System.Collections.Generic;

namespace PersistedSortedList.Tests
{
    public class NewIndexReader 
    {
        private const int DefaultFreeListSize = 32;

        private readonly object _lockObject;
        private readonly List<NewNode> _freelist;

        public NewIndexReader()
        {
            _lockObject = new object();
            _freelist = new List<NewNode>(DefaultFreeListSize);
        }
        
        public NewNode NewNode()
        {
            lock (_lockObject)
            {
                var index = _freelist.Count - 1;

                if (index < 0)
                {
                    return new NewNode(this);
                }

                var n = _freelist[index];

                _freelist[index] = null;
                _freelist.RemoveAt(index);

                return n;
            }
        }
    }
}