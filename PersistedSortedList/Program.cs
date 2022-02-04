using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace PersistedSortedList
{
    class Program
    {
        static void Main(string[] args)
        {
            File.Delete("t1");
            File.Delete("repository.db");
            File.Delete("repository.index");
            Thread.Sleep(1000);
            FunctionTest();
            //PerformenceTest();
        }

        private static void PerformenceTest()
        {

            var count = 1000;
            TestPersistedList(count);

            TestSortedList(count);
        }

        private static void TestSortedList(int count)
        {
            var rnd = new Random();
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            var t1 = File.Create("t1");
            for (int i = 0; i < count; i++)
            {
                var value = rnd.NextDouble().ToString();
                t1.Write(JsonSerializer.SerializeToUtf8Bytes(new TestObject { Value = value }));
            }

            t1.Close();
            Console.Out.WriteLine($"Adding {count} took {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void TestPersistedList(int count)
        {
            var rnd = new Random();
            using var list = new PersistedSortedList<TestObject>("perf");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                list.Add(new TestObject { Value = rnd.NextDouble().ToString() });
            }

            Console.Out.WriteLine($"Adding {count} took {stopwatch.ElapsedMilliseconds}ms");
        }

        static void FunctionTest()
        {
            //var index = new Index<TestObject>(object=>Value)
            //Index.Add(new TestObject { Value = "H" })

            using (var list = new PersistedSortedList<TestObject>("repository"))
            {
                list.Add(new TestObject { Value = "H" });
                list.Add(new TestObject { Value = "C" });
                list.Add(new TestObject { Value = "B" });
                list.Add(new TestObject { Value = "A", Extra = "Testar A" });
                list.Add(new TestObject { Value = "D" });
                list.Add(new TestObject { Value = "E" });
                list.Add(new TestObject { Value = "F" });
                list.Add(new TestObject { Value = "G" });
                var found = list.Get(new TestObject { Value = "A" });
                Console.Out.WriteLine("Search:" + JsonSerializer.Serialize(found));
            }

            using var file = new FileAdapter("repository.db");
            var position = 0;
            byte[] b;
            while ((b = file.ReadLine(position)).Length != 0)
            {
                Console.Out.WriteLine(position.ToString("X8") + " " + Encoding.UTF8.GetString(b));
                position += b.Length;
            }


            using var file1 = new FileAdapter("repository.index");
            position = 0;
            while ((b = file1.Read(position, 101)) != null)
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
            return Value.CompareTo(((TestObject)obj).Value);
        }
    }
}
