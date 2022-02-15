using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PersistedSortedList.Tests.Tests
{
    [TestClass()]
    public class BTreeTests
    {
        private NewBTree<int> tr;

        [TestInitialize]
        public void Init()
        {
            var repositoryMock = new Mock<IRepository<int>>();
            repositoryMock.Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int i) => i);
            var indexReader = new NewIndexReader<int>(repositoryMock.Object, new Mock<IFileAdapter>().Object);
            tr = new NewBTree<int>(indexReader);
        }

        [TestMethod]
        public void ReplaceOrInsertTest()
        {
            tr.Add(1);
            tr.Add(2);
            tr.Add(3);
            tr.Add(4);
            tr.Add(5);
            tr.Add(6);
            tr.Add(7);

            var node = tr.Get(4);
            Assert.AreEqual("4", node.ToString());
        }

        [TestMethod]
        public void ReplaceOrInsertComplexTest()
        {
            var repositoryMock = new Mock<IRepository<TestObject>>();
            repositoryMock
                .Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int i) => new TestObject{Value = i.ToString(), Extra = $"Extra{i}"});
            var indexReader = new NewIndexReader<TestObject>(repositoryMock.Object, new Mock<IFileAdapter>().Object);

            var tr = new NewBTree<TestObject>(indexReader);

            tr.Add(1);
            tr.Add(2);
            tr.Add(3);
            tr.Add(4);
            tr.Add(5);
            tr.Add(6);
            tr.Add(7);

            var node = tr.Get(new TestObject {Value = 4.ToString()});
            Assert.AreEqual("4", node.Value);
            Assert.AreEqual("Extra4", node.Extra);
        }
    }
}