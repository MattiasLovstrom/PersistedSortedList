using System;
using System.Collections.Generic;
using System.IO;

namespace PersistedSortedList
{
    public class FileAdapter : IFileAdapter, IDisposable
    {
        private readonly string _fileName;
        private readonly FileStream _file;

        public FileAdapter(string fileName)
        {
            _fileName = fileName;
            if (!File.Exists(fileName))
            {
                _file = File.Create(fileName);
                _file.WriteByte(13);
            }
            else
            {
                _file = File.Open(fileName, FileMode.Open);
            }
        }

        public byte[] Read(long position, long length)
        {
            try
            {
                _file.Position = position;
                var buffer = new byte[length];
                var read = _file.Read(buffer, 0, buffer.Length);
                
                return read == length ? buffer : null; 
            }
            finally
            {
                Last = _file.Length;
            }
        }

        public long Write(long position, byte[] buffer)
        {
            try
            {
                _file.Position = position;
                _file.Write(buffer);
                _file.WriteByte(13);

                return _file.Position;
            }
            finally
            {
                Last = _file.Length;
            }
        }

        public byte[] ReadLine(int position)
        {
            var buffer = new List<byte>();
            try
            {
                _file.Position = position;
                int b;
                do
                {
                    b = _file.ReadByte();
                    if (b < 0) return buffer.ToArray();
                    buffer.Add((byte)b);
                } while (b != 13);

                return buffer.ToArray();
            }
            finally
            {
                Last = _file.Length;
            }
        }

        public long Last { get; set; }

        public void Dispose()
        {
            if (_file != null)
            {
                _file.Close();
                _file.Dispose();
            }
        }
    }
}