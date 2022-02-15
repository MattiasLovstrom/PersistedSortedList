using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace PersistedSortedList.Tests
{
    public class NewIndexReader<T> : INewIndexReader<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private readonly IFileAdapter _indexFile;
        private readonly List<NewNode<T>> _list;
        private MemoryCache _cache;

        public NewIndexReader(
            IRepository<T> repository,
            IFileAdapter indexFile)
        {
            _repository = repository;
            _indexFile = indexFile;
            _list = new List<NewNode<T>>();
            _cache = MemoryCache.Default;
        }

        public NewNode<T> NewNode()
        {
            var node = new NewNode<T>(this, _repository)
            {
                Position = (int)_indexFile.Last
            };
            _cache.Add(new CacheItem(node.Position.ToString(), node), new CacheItemPolicy {SlidingExpiration = TimeSpan.FromMinutes(1)});
            _indexFile.Write(_indexFile.Last, Serialize(node));
            
            return node;
        }

        public NewNode<T> Get(int reference)
        {
            if (_cache.Get(reference.ToString()) is NewNode<T> cached)
            {
                cached.Position = reference;
                return cached;
            }

            var buffer = _indexFile.Read(reference, Constants.BranchingFactor);
            var node = Deserialize(buffer);
            node.Position = reference;

            return node;
        }

        public void Update(NewNode<T> node)
        {
            var serialize = Serialize(node);
            Console.Out.WriteLine("Update " + node.Position.ToString("X8") + " " + Encoding.UTF8.GetString(serialize) + " " + node);
            _indexFile.Write(node.Position, serialize);
            _cache.Add(new CacheItem(node.Position.ToString(), node), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(1) });
        }

        private NewNode<T> Deserialize(byte[] block)
        {
            var node = new NewNode<T>(this, _repository);
            var items = Encoding.UTF8.GetString(block).TrimStart('[').TrimEnd(']').Split(',');
            var references = items.Select(i => int.Parse(i, NumberStyles.HexNumber)).ToArray();

            node.Items = new List<int>(references.Take(Constants.BranchingFactor));
            node.Children = new List<int>(references.Skip(Constants.BranchingFactor + 1));

            return node;
        }

        public static byte[] Serialize(NewNode<T> node)
        {
            var serialized = new StringBuilder();
            serialized.Append("[");
            for (var i = 0; i < Constants.BranchingFactor; i++)
            {
                if (i < node.Items.Count)
                {
                    serialized.Append(node.Items[i].ToString("X8")).Append(",");
                }
                else
                {
                    serialized.Append(0.ToString("X8")).Append(",");
                }
            }
            for (var i = 0; i < Constants.BranchingFactor + 1; i++)
            {
                if (i < node.Children.Count)
                {
                    serialized.Append(node.Children[i].ToString("X8")).Append(",");
                }
                else
                {
                    serialized.Append(0.ToString("X8")).Append(",");
                }
            }

            serialized.Remove(serialized.Length -1, 1);
            serialized.Append("]");

            return Encoding.UTF8.GetBytes(serialized.ToString());
        }
    }
}