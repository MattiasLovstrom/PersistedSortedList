using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// ReSharper disable once CheckNamespace
namespace SortedFileList.Tests
{
    [TestClass()]
    public class IndexReaderTests
    {
        [TestMethod()]
        public void GetOrCreateEmptyTest()
        {
            var testObject = new IndexReader("",
                new Mock<IFileAdapter>().Object);
            testObject.Create();

        }
    }
}