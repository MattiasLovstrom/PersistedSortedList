using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace PersistedSortedList.Tests
{
    [TestClass]
    public class BTreeTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var node = new Node { Values = { [0] = 10 } };
            var deserializeNode = Node.DeserializeNode(Node.Serialize(node));

            Assert.AreEqual(node.Values[0], deserializeNode.Values[0]);
        }
    }
}
