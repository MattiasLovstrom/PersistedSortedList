using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistedSortedList.Tests;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;

namespace PersistedSortedList.Tests.Tests
{
    [TestClass]
    public class NewNodeTests
    {
        private NewNode<int> _testObject;

        [TestInitialize]
        public void Init()
        {
            var repositoryMock = new Mock<IRepository<int>>();
            repositoryMock.Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int i) => i);
            var indexReader = new NewIndexReader<int>(repositoryMock.Object);

            _testObject = new NewNode<int>(
                indexReader,
                repositoryMock.Object);
        }

        [TestMethod]
        public void TryGetValueTest()
        {
            _testObject.Items = new List<int>{0,1,2};
            _testObject.TryGetValue(1, out var index);

            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void TryGetNonExistingValueTest()
        {
            _testObject.Items = new List<int> { 0, 1, 4 };
            Assert.IsFalse(_testObject.TryGetValue(3, out var index));

            Assert.AreEqual(3, index);
        }
    }
}