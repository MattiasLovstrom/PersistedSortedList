using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PersistedSortedList
{
    public class BTree
    {
        public const int BranchingFactor = 5;
        public const int NodeLength = 100;

        private IComparer _comparer;
        public IIndexReader _nodeReader;
        private Node root;

        public BTree(IComparer comparer,
            IIndexReader nodeReader)
        {
            _comparer = comparer;
            _nodeReader = nodeReader;
            root = nodeReader.Create();
        }

        public Node Add(int value)
        {
            return Add(value, root);
        }

        public Node Add(int value, Node current)
        {
            int i;
            for (i = 0; i < current.Values.Length; i++)
            {
                if (current.Values[i] == 0)
                {
                    current.Values[i] = value;
                    _nodeReader.Update(current);
                    return current;
                }
                if (_comparer.Compare(value, current.Values[i]) <= 0) break;
            }

            if (current.References[i] == 0)
            {
                var newNode = _nodeReader.Create();
                current.References[i] = newNode.Position;
                _nodeReader.Update(current);
                return Add(value, newNode);
            }
            return Add(value, _nodeReader.Get(current.References[i]));
        }

        public T Get<T>(T prototype)
        {
            return Get(prototype, root);
        }


        public T Get<T>(T value, Node current)
        {
            int i;
            for (i = 0; i < current.Values.Length; i++)
            {
                //todo don't return the prototype
                if (_comparer.Compare(value, current.Values[i]) == 0) return value;
                if (_comparer.Compare(value, current.Values[i]) <= 0) break;
            }

            return Get(value, _nodeReader.Get(current.References[i]));
        }


        public static byte[] Serialize(Node node)
        {
            var serialized = new StringBuilder();
            serialized.Append("[");
            serialized.Append(node.Values.Select(v => v.ToString("X8")).Aggregate((c, n) => c + "," + n));
            serialized.Append(",");
            serialized.Append(node.References.Select(v => v.ToString("X8")).Aggregate((c, n) => c + "," + n));
            serialized.Append("]");

            return Encoding.UTF8.GetBytes(serialized.ToString());
        }

        public static Node DeserializeNode(byte[] block)
        {
            var node = new Node();
            var items = Encoding.UTF8.GetString(block).TrimStart('[').TrimEnd(']').Split(',');
            var references = items.Select(i => Int32.Parse(i, NumberStyles.HexNumber)).ToArray();

            Array.Copy(references, node.Values, BranchingFactor);
            Array.Copy(references, BranchingFactor, node.References, 0, BranchingFactor + 1);

            return node;
        }
    }
}