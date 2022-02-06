using System;
using System.Runtime.Caching;

namespace PersistedSortedList
{
    public class IndexReader : IIndexReader
    {
        private readonly IFileAdapter _fileAdapter;
        private readonly MemoryCache _cache;

        public IndexReader(
            IFileAdapter fileAdapter,
            MemoryCache cache)
        {
            _cache = cache;
            _fileAdapter = fileAdapter;
        }

        public Node Create(int parentPosition)
        {
            var node = new Node
            {
                Parent = parentPosition,
                Position = (int)_fileAdapter.Last
            };

            _fileAdapter.Write(_fileAdapter.Last, Node.Serialize(node));
            _cache.Add(node.Position.ToString(), node, new CacheItemPolicy {SlidingExpiration = TimeSpan.FromHours(1)});
            
            return node;
        }

        public Node Get(int reference)
        {
            if (_cache.Get(reference.ToString()) is Node node)
            {
                return node;
            }
            var buffer = _fileAdapter.Read(reference, Node.NodeLength);
            node = Node.DeserializeNode(buffer);
            node.Position = reference;

            return node;
        }

        public void Update(Node node)
        {
            _fileAdapter.Write(node.Position, Node.Serialize(node));
            _cache.Remove(node.Position.ToString());
            _cache.Add(node.Position.ToString(), node, new CacheItemPolicy {SlidingExpiration = TimeSpan.FromHours(1)});
        }
    }
}