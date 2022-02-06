using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistedSortedList;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;

namespace PersistedSortedList.Tests
{
    [TestClass()]
    public class IndexTests
    {
        private Index<TestObject> _testObject;

        [TestInitialize]
        public void Init()
        {
            Node.BranchingFactor = 2;
            var repositoryMock = new Mock<IRepository<TestObject>>();
            repositoryMock.Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int p) => new TestObject { Value = p.ToString() });

            _testObject = new Index<TestObject>(
                new Mock<IIndexReader>().Object,
                repositoryMock.Object);
        }

        [TestMethod]
        public void AddFirstTest()
        {
            var node = _testObject.Add(
                1,
                new TestObject { Value = "1" },
                new Node { });

            Assert.AreEqual(1, node.Values[0]);
        }

        [TestMethod]
        public void AddSecondTest()
        {
            var node = _testObject.Add(
                2,
                new TestObject { Value = "2" },
                new Node { Values = new []{1,0}});

            Assert.AreEqual(2, node.Values[1]);
        }

        [TestMethod]
        public void AddThirdTest()
        {
            var node = _testObject.Add(
                3,
                new TestObject { Value = "3" },
                new Node { Values = new[] { 1, 2 } });

            Assert.AreEqual(3, node.Values[0]);
        }

        // 3 => [1,2] => [2,] => [1,], [3,]
        [TestMethod]
        public void SplitTest()
        {
            Assert.Fail();
            //var current = _testObject.Split(
            //    new Node {Values = new [] {1, 2}},
            //    new TestObject{Value = "3"});


            //Assert.AreEqual(2, current.Values[0]);
            //Assert.AreEqual(0, current.Values[1]);
            //Assert.IsTrue(current.References[0] != 0);
            //Assert.IsTrue(current.References[1] != 0);
            //Assert.IsTrue(current.References[2] == 0);
        }
    }
}