using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

// ReSharper disable once CheckNamespace
namespace SortedFileList.Tests
{
    [TestClass()]
    public class PersistedSortedListTests
    {
        private PersistedSortedList<TestItem> _testObject;
        private Mock<IIndex<TestItem>> _indexMock;
        private Mock<IRepository<TestItem>> _repositoryMock;

        [TestInitialize]
        public void Init()
        {
            _repositoryMock = new Mock<IRepository<TestItem>>();
            _indexMock = new Mock<IIndex<TestItem>>();
            _testObject = new PersistedSortedList<TestItem>(
                _repositoryMock.Object,
                _indexMock.Object);
        }
        
        [TestMethod]
        public void AddTest()
        {
            var testItem = new TestItem {Value = "1"};

            _testObject.Add(testItem);
            _repositoryMock.Verify(repository=>repository.Add(It.IsAny<TestItem>()));
            _indexMock.Verify(index=>index.Add(It.IsAny<long>()));
        }
    }

    public class TestItem : IComparable
    {
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public string Value { get; set; }
    }
}