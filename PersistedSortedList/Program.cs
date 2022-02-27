using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using PersistedSortedList.Tests;

namespace PersistedSortedList
{
    class Program
    {
        static void Main(string[] args)
        {
            File.Delete("t1");
            File.Delete("repository.db");
            File.Delete("repository.index");
            FunctionTest();
            //PerformenceTest();
        }

        static void FunctionTest()
        {
            using (var list = new PersistedSortedList<TestObject>("repository"))
            {
                list.Add(new TestObject { Value = "1" });
                list.Add(new TestObject { Value = "2" });
                list.Add(new TestObject { Value = "3" });
                list.Add(new TestObject { Value = "4", Extra = "Testar A" });
                list.Add(new TestObject { Value = "5" });
                list.Add(new TestObject { Value = "6" });
                list.Add(new TestObject { Value = "7" });
            }

            using (var list1 = new PersistedSortedList<TestObject>("repository"))
            {
                var found = list1.Get(new TestObject { Value = "1" });
                Console.Out.WriteLine("Search:" + JsonSerializer.Serialize(found));
                found = list1.Get(new TestObject { Value = "7" });
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
            while ((b = file1.Read(position, Constants.NodeLength + 1)) != null)
            {
                Console.Out.WriteLine(position.ToString("X8") + " " + Encoding.UTF8.GetString(b));
                position += b.Length;
            }
        }

        private static void PerformenceTest()
        {

            var count = 100;
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
            string testValue = "";
            var rnd = new Random();
            var stopwatch = new Stopwatch();
            using (var list = new PersistedSortedList<TestObject>("perf"))
            {
                stopwatch.Start();
                for (int i = 0; i < count; i++)
                {
                    var value = rnd.NextDouble().ToString();
                    if (i == count / 2) testValue = value;
                    list.Add(new TestObject { Value = value });
                }
                Console.Out.WriteLine($"Adding {count} took {stopwatch.ElapsedMilliseconds}ms");
            }

            using (var list = new PersistedSortedList<TestObject>("perf"))
            {
                stopwatch.Restart();
                var n = list.Get(new TestObject { Value = testValue });
                Console.Out.WriteLine($"Search for {testValue} find {n.Value} took {stopwatch.ElapsedMilliseconds}ms");
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

        public override string ToString()
        {
            return Value;
        }
    }
}
