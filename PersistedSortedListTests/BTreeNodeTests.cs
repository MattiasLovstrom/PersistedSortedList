using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortedFileList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SortedFileList.Tests
{


    public class Comparer: IComparer
    {
        private readonly ILoader _loader;

        public Comparer(
            ILoader loader)
        {
            _loader = loader;
        } 
        public int Compare(object? x, object? y)
        {
            var x1 = _loader.Get((int)x);
            var y1 = _loader.Get((int)y);
            return x1.CompareTo(y1);
        }
    }

    public interface ILoader
    {
        string Get(int value);
    }

    public class Loader : ILoader
    {
        public string Get(int value)
        {
            if (value == -1020) return "A";

            return "B";
        }
    }
}