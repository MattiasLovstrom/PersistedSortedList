using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PersistedSortedList
{
    class Program
    {
        static void Main(string[] args)
        {
            File.Delete("repository.db");
            File.Delete("repository.index");
            var list = new PersistedSortedList<TestObject>("repository");
            list.Add(new TestObject { Value = "H" });
            list.Add(new TestObject { Value = "C" });
            list.Add(new TestObject { Value = "B" });
            list.Add(new TestObject { Value = "A", Extra="Testar A" });
            list.Add(new TestObject { Value = "D" });
            list.Add(new TestObject { Value = "E" });
            list.Add(new TestObject { Value = "F" });
            list.Add(new TestObject { Value = "G" });

            var found = list.Get(new TestObject { Value = "A" });
            Console.Out.WriteLine("Search:" + JsonSerializer.Serialize(found));
            var file = new FileAdapter("repository.db");
            var position = 0;
            byte[] b;
            while ((b = file.ReadLine(position)).Length != 0)
            {
                Console.Out.WriteLine(position.ToString("X8")+ " "+ Encoding.UTF8.GetString(b));
                position += b.Length;
            }

            file = new FileAdapter("repository.index");
            position = 0;
            while ((b = file.Read(position, 101)) != null)
            {
                Console.Out.WriteLine(position.ToString("X8") + " " + Encoding.UTF8.GetString(b));
                position += b.Length;
            }
        }
    }

    public class TestObject : IComparable
    {
        [Key]
        public string Value { get; set; }
        public string Extra { get; set; }

        public int CompareTo(object? obj)
        {
            return Value.CompareTo(((TestObject) obj).Value);
        }
    }
}
