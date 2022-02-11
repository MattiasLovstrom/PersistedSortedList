using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PersistedSortedList.Tests.Tests
{
    [TestClass()]
    public class BTreeTests
    {
        [TestMethod]
        public void ReplaceOrInsertTest()
        {
            var repositoryMock = new Mock<IRepository<int>>();
            repositoryMock.Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int i) => i);
            var indexReader = new NewIndexReader<int>(repositoryMock.Object);
            var tr = new NewBTree<int>(2,
                indexReader, 
                repositoryMock.Object);

            tr.ReplaceOrInsert(1);
            tr.ReplaceOrInsert(2);
            tr.ReplaceOrInsert(3);
            tr.ReplaceOrInsert(4);
            tr.ReplaceOrInsert(5);
            tr.ReplaceOrInsert(6);
            tr.ReplaceOrInsert(7);

            var node = tr.Get(4);
            Assert.AreEqual("4", node.ToString());
        }

        [TestMethod]
        public void ReplaceOrInsertComplexTest()
        {
            var repositoryMock = new Mock<IRepository<TestObject>>();
            repositoryMock.Setup(repository => repository.Get(It.IsAny<int>()))
                .Returns((int i) => new TestObject{Value = i.ToString(), Extra = $"Extra{i}"});
            var indexReader = new NewIndexReader<TestObject>(repositoryMock.Object);
            var tr = new NewBTree<TestObject>(2,
                indexReader,
                repositoryMock.Object);

            tr.ReplaceOrInsert(1);
            tr.ReplaceOrInsert(2);
            tr.ReplaceOrInsert(3);
            tr.ReplaceOrInsert(4);
            tr.ReplaceOrInsert(5);
            tr.ReplaceOrInsert(6);
            tr.ReplaceOrInsert(7);

            var node = tr.Get(4);
            Assert.AreEqual("4", node.Value);
            Assert.AreEqual("Extra4", node.Extra);
        }

    }
}