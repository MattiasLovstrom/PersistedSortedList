using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PersistedSortedList
{
    public class Node
    {
        public const int NodeLength = 100;
        public const int BranchingFactor = 5;
        public int Position { get; set; }
        public int[] Values = new int[BranchingFactor];
        public int[] References = new int[BranchingFactor + 1];

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

        [DebuggerStepThrough]
        public override string ToString()
        {
            return Encoding.UTF8.GetString(Node.Serialize(this));
        }
    }
}