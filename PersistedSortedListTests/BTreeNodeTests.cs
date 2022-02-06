using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistedSortedList;
using System.Collections;
using Castle.DynamicProxy.Generators.Emitters;

namespace PersistedSortedList.Tests
{
    [TestClass()]
    public class BTreeNodeTests
    {
        [TestMethod()]
        public void AddTest()
        {
            var root = new BTreeNode(null);

            root.Add(1);
            Assert.AreEqual(1, root.Values[0]);
            root.Add(2);
            Assert.AreEqual(2, root.Values[1]);
            root = root.Add(3) ?? root;
            Assert.AreEqual(3, root.References[1].Values[0]);
            root = root.Add(4);
            Assert.AreEqual(4, root.References[1].Values[1]);
            root = root.Add(5);
            Assert.AreEqual(5, root.References[2].Values[0]);
            root = root.Add(6);
            Assert.AreEqual(6, root.References[2].Values[1]);
            root = root.Add(7) ?? root;
            Assert.AreEqual(7, root.References[1].References[1].Values[0]);
        }

        [TestMethod()]
        public void SearchTest()
        {
            var root = new BTreeNode(null);
            Assert.AreSame(root, root.Search(root, 1));
        }

        [TestMethod()]
        public void SearchLevel2Test()
        {
            var node1 = new BTreeNode(null){ Values = new []{1}};
            var node3 = new BTreeNode(null){ Values = new []{3}};
            var root = new BTreeNode(null)
            {
                Values = new []{2},
                References = new []
                {
                    node1, 
                    node3, 
                }
            };
            Assert.AreSame(node3, root.Search(root, 3));
            Assert.AreSame(node3, root.Search(root, 4));
        }

        [TestMethod]
        public void InsertValueTest()
        {
            var node = new BTreeNode(null){Values = new []{1,2,4,0}};
            var pos = node.InsertValue(3);
            Assert.AreEqual(2, pos);
            Assert.AreEqual(3, node.Values[2]);
            Assert.AreEqual(4, node.Values[3]);
        }
    }
}

// ReSharper disable once CheckNamespace
namespace SortedFileList.Tests
{
    public class Comparer: IComparer
    {
        private readonly ILoader _loader;

        public Comparer(
            ILoader loader)
        {
            _loader = loader;
        } 
        public int Compare(object? x, object? y)
        {
            var x1 = _loader.Get((int)x);
            var y1 = _loader.Get((int)y);
            return x1.CompareTo(y1);
        }
    }

    public interface ILoader
    {
        string Get(int value);
    }

    public class Loader : ILoader
    {
        public string Get(int value)
        {
            if (value == -1020) return "A";

            return "B";
        }
    }
}