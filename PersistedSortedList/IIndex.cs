using System;

namespace SortedFileList
{
    public interface IIndex<T>
    {
        void Add(long fileReference);
        T Get(T prototype);
    }
}