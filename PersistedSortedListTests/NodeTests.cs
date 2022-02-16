using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistedSortedList.Tests;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;

namespace PersistedSortedList.Tests.Tests
{
    [TestClass]
    public class NodeTests
    {
        private Node<int> _testObject;

        [TestInitialize]
        public void Init()
        {
            var repositoryMock = new Mock<IRepository<int>>();
            repositoryMock.Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int i) => i);
            var indexReader = new IndexReader<int>(repositoryMock.Object, new Mock<IFileAdapter>().Object);

            _testObject = new Node<int>(
                indexReader,
                repositoryMock.Object);
        }

        [TestMethod]
        public void TryGetValueTest()
        {
            _testObject.Items = new List<int> { 0, 1, 2 };
            _testObject.TryGetIndexOfReference(1, out var index);

            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void TryGetNonExistingValueTest()
        {
            _testObject.Items = new List<int> { 0, 1, 4 };
            Assert.IsFalse(_testObject.TryGetIndexOfReference(3, out var index));

            Assert.AreEqual(2, index);
        }
    }
}