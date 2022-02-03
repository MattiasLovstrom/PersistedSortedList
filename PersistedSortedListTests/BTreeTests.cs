using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SortedFileList;

// ReSharper disable once CheckNamespace
namespace PersistedSortedList.Tests
{
    [TestClass()]
    public class BTreeTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var node = new Node() { Values = { [0] = 10 } };
            var deserializeNode = Node.DeserializeNode(Node.Serialize(node));

            Assert.AreEqual(node.Values[0], deserializeNode.Values[0]);
        }

        [TestMethod]
        public void AddTest()
        {
            var mock = new Mock<IIndexReader>();

            var testObject = new BTree<int>(mock.Object, 
                new Mock<IRepository<int>>().Object);
            
            var root = new Node();
            testObject.Add(2, 2, root);
            mock.Verify(reader=>reader.Update(It.IsAny<Node>()));
            mock
                .Setup(reader => reader.Create())
                .Returns(new Node());
            testObject.Add(1, 1, root);
            mock.Verify(reader => reader.Create());

        }
    }
}
