using System;

namespace PersistedSortedList
{
    public interface IFileAdapter : IDisposable
    {
        byte[] Read(long position, long length);
        long Write(long position, byte[] buffer);
        byte[] ReadLine(int position);
        long Last { get; set; }
    }
}