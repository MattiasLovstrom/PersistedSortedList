using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace SortedFileList
{
    public class FileAdapter : IFileAdapter
    {
        private readonly string _fileName;

        public FileAdapter(string fileName)
        {
            _fileName = fileName;
            if (!File.Exists(fileName))
            {
                using var file = File.Create(fileName);
                file.Close();
            }
        }

        public byte[] Read(long position, long length)
        {
            var file = File.OpenRead(_fileName);
            try
            {
                file.Position = position;
                var buffer = new byte[length];
                var read = file.Read(buffer, 0, buffer.Length);
                
                return read == length ? buffer : null; 
            }
            finally
            {
                Last = file.Length;
                file.Close();
            }
        }

        public long Write(long position, byte[] buffer)
        {
            using var file = File.OpenWrite(_fileName);
            try
            {
                Console.Out.Write(position.ToString("X") + "-");
                file.Position = position;
                file.Write(buffer);
                file.WriteByte(13);
                Console.Out.WriteLine(file.Position.ToString("X"));

                return file.Position;
            }
            finally
            {
                Last = file.Length;
                file.Close();
            }
        }

        public byte[] ReadLine(int position)
        {
            var buffer = new List<byte>();
            using var file = File.OpenRead(_fileName);
            try
            {
                file.Position = position;
                int b;
                do
                {
                    b = file.ReadByte();
                    if (b < 0) return buffer.ToArray();
                    buffer.Add((byte)b);
                } while (b != 13);

                return buffer.ToArray();
            }
            finally
            {
                Last = file.Length;
                file.Close();
            }
        }

        public long Last { get; set; }
        
        public string Display()
        {
            var content = new StringBuilder();
            foreach (var line in File.ReadAllLines(_fileName))
            {
                content.AppendLine(line);
            }

            return content.ToString();
        }
    }
}