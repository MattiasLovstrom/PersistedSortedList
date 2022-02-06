using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PersistedSortedList
{
    // 1
    //
    // 1 2
    // 
    // 123 aplit
    //   2
    // 1   3
    //
    //   2
    // 1   34
    //
    //   2
    // 1   345 split
    //   24
    // 1   3 5 split
    //
    //   24
    // 1   3 56 
    //
    //  2 4
    // 1 3 567 split 6 => 5,7 
    //  2 4 6 split
    // 1 3 5 7
    //    4  
    //  2   6 
    // 1 3 5 7
    public class BTreeNode
    {
        public static int BranchingFactor = 2;
        public BTreeNode Parent { get; set; }
        public int[] Values = new int[BranchingFactor + 1];
        public BTreeNode[] References = new BTreeNode[BranchingFactor + 2];

        public BTreeNode(BTreeNode parent)
        {
            Parent = parent;
        }

        public BTreeNode(BTreeNode parent, IEnumerable<int> values)
        {
            Parent = parent;
            var i = 0;
            foreach (var value in values)
            {
                Values[i++] = value;
            }
        }

        public BTreeNode Add(int value)
        {
            return Add(Search(this, value), value);
        }

        public BTreeNode Add(BTreeNode value)
        {
            for (var i = 0; i < value.Values.Length; i++)
            {
                var v = value.Values[i];
                if (v == 0) break;
                var position = InsertValue(v);
                References[position] = value.References[i];
                References[position + 1] = value.References[i + 1];
            }

            return this;
        }

        public BTreeNode Add(BTreeNode current, int value)
        {
            current.InsertValue(value);
            if (current.Values[BranchingFactor] == 0) return current;
            var median = current.Values.Length / 2;
            var parent = current.Parent ?? new BTreeNode(null);
            var separationNode = new BTreeNode(null, new[] {Values[median]});
            separationNode.References = new[]
            {
                new BTreeNode(separationNode, current.Values.Take(median).ToArray()),
                new BTreeNode(separationNode, current.Values.Skip(median + 1).ToArray())
            };

            return parent.Add(separationNode);
        }

        public int InsertValue(int value)
        {
            var i = 0;
            for (; i < Values.Length; i++)
            {
                if (Values[i] == 0 || Values[i] > value) break;
            }
            var position = i;
            Array.Copy(Values, position, Values, position + 1, Values.Length - position - 1);
            Values[position] = value;
            return position;
        }

        //private BTreeNode Split(BTreeNode current, in int value)
        //{
        //    var all = new SortedList<int, int> {{value, value}};
        //    foreach (var v in current.Values)
        //    {
        //        all.Add(v,v);
        //    }

        //    var medianIndex = (BranchingFactor + 1) / 2;
        //    var leftValues = new List<int>();
        //    for (int i = 0; i < medianIndex; i++)
        //    {
        //        leftValues.Add(all[i]);
        //    }
        //    var left = new BTreeNode
        //    {
        //        Parent = current,
        //        Values = leftValues.ToArray()
        //    };
        //    var rightValues = new List<int>();
        //    for (var i = medianIndex; i < BranchingFactor + 1 ; i++)
        //    {
        //        rightValues.Add(all[i]);
        //    }
        //    var right = new BTreeNode
        //    {
        //        Parent = current,
        //        Values = rightValues.ToArray()
        //    };
        //    Add(current.Parent, all[medianIndex]);
        //}

        public BTreeNode Search(BTreeNode current, int value)
        {
            var i = 0;
            for (; i < Values.Length; i++)
            {
                if (current.Values[i] == value)
                {
                    return current;
                }
                if (current.Values[i] == 0 || value < current.Values[i]) break;
            }
            return current.References[i] != null
                ? Search(current.References[i], value)
                : current;
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append(Values.Select(v => v.ToString()).Aggregate((c, n) => c + "," + n));
            return message.ToString();
        }
    }
}
