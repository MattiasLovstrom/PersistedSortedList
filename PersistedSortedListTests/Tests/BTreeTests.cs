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
            var tr = new NewBTree<int>(2);
            tr.ReplaceOrInsert(1);
            tr.ReplaceOrInsert(2);
            tr.ReplaceOrInsert(3);
            tr.ReplaceOrInsert(4);
            tr.ReplaceOrInsert(5);
            tr.ReplaceOrInsert(6);
            tr.ReplaceOrInsert(7);
            //tr.ReplaceOrInsert(8);
            //tr.ReplaceOrInsert(9);
            //tr.ReplaceOrInsert(10);

            var node = tr.Get(4);
            Assert.AreEqual("4", node.ToString());
        }
    }
}