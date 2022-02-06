using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistedSortedList.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersistedSortedList.Tests.Tests
{
    [TestClass()]
    public class BTreeTests
    {
        [TestMethod()]
        public void ReplaceOrInsertTest()
        {
            /// BTree(2), for example, will create a 2-3-4 tree (each node contains 1-3 items
            /// and 2-4 children).
            var tr = new BTree<Int>(2, new IntComparer());
            tr.ReplaceOrInsert(new Int(1));
            tr.ReplaceOrInsert(new Int(2));
            tr.ReplaceOrInsert(new Int(3));
            tr.ReplaceOrInsert(new Int(4));
            tr.ReplaceOrInsert(new Int(5));
            tr.ReplaceOrInsert(new Int(6));
            tr.ReplaceOrInsert(new Int(7));
            tr.ReplaceOrInsert(new Int(8));
            tr.ReplaceOrInsert(new Int(9));
            tr.ReplaceOrInsert(new Int(10));

            var node = tr.Get(new Int(4));
        }
    }

    public class IntComparer : Comparer<Int>
    {
        public override int Compare(Int x, Int y)
        {
            return x.CompareTo(y);
        }
    }
}