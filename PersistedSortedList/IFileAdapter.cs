namespace PersistedSortedList
{
    public interface IFileAdapter
    {
        byte[] Read(long position, long length);
        long Write(long position, byte[] buffer);
        byte[] ReadLine(int position);
        long Last { get; set; }
    }
}