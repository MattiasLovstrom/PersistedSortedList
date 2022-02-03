using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortedFileList;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;

namespace SortedFileList.Tests
{
    [TestClass()]
    public class IndexReaderTests
    {
        // 
        [TestMethod()]
        public void GetOrCreateEmptyTest()
        {
            var testObject = new IndexReader("",
                new Mock<IFileAdapter>().Object);
            testObject.Create();

        }

        [TestMethod()]
        public void GetOrCreateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }
    }
}