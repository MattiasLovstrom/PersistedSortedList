﻿using System;

namespace PersistedSortedList.Tests
{
    public interface INewIndexReader<T> where T : IComparable
    {
        NewNode<T> NewNode();
    }
}