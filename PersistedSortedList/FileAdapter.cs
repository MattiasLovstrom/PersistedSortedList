using System;
using System.Collections.Generic;
using System.IO;

namespace PersistedSortedList
{
    public class FileAdapter : IFileAdapter
    {
        private readonly string _fileName;
        public FileStream _file;

        public FileAdapter(string fileName)
        {
            _fileName = fileName;
            if (!File.Exists(fileName))
            {
                _file = File.Create(fileName);
                Console.Out.WriteLine("Create: " + _file.Name);
            }
            else
            {
                _file = File.Open(fileName, FileMode.Open);
                Console.Out.WriteLine("Open: " + _file.Name);
                while(true)
                {
                    try
                    {
                        _file.Position = 0;
                        break;
                    }
                    catch
                    {
                        Console.Out.WriteLine($"Can't open {_file.Name}, retrying");
                    }
                }

            }
            Console.Out.WriteLine("file " + _file.GetType().Name + " " + _file.GetHashCode());
        }

        public byte[] Read(long position, long length)
        {
            Console.Out.WriteLine("File" + _file);
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
            Console.Out.WriteLine("File" + _file);
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
            Console.Out.WriteLine("File" + _file);
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
                Console.Out.WriteLine("Close: " + _file.Name);
                _file.Flush(true);
                _file.Close();
                _file.Dispose();
                _file = null;
            }
        }
    }
}