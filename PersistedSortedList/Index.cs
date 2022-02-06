using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistedSortedList
{
    public class Index<T> : IIndex<T> where T : IComparable
    {
        private readonly IRepository<T> _repository;
        private readonly IIndexReader _indexReader;
        private readonly Node _root;

        public Index(
            IIndexReader indexReader,
            IRepository<T> repository)
        {
            _indexReader = indexReader;
            _repository = repository;
            _root = _indexReader.Create(0);
        }

        public void Add(long fileReference, T value)
        {
            Add((int)fileReference, value, _root);
        }

        public Node Add(int position, T value, Node current)
        {
            int i;
            for (i = 0; i < current.Values.Length; i++)
            {
                if (current.Values[i] == 0)
                {
                    current.Values[i] = position;
                    _indexReader.Update(current);
                    return current;
                }

                var v = _repository.Get(current.Values[i]);
                if (value.CompareTo(v) <= 0) break;
            }


            if (current.References[i] != 0)
            {
                return Add(position, value, _indexReader.Get(current.References[i]));
            }

            var newNode = _indexReader.Create(current.Position);
            current.References[i] = newNode.Position;
            _indexReader.Update(current);
            return Add(position, value, newNode);

        }

        public Node Split(int position, T value, Node current)
        {
            var values = new T[Node.BranchingFactor + 1];
            var count = 0;
            foreach (var currentValue in current.Values)
            {
                var v = _repository.Get(currentValue);
                values[count++] = v.CompareTo(value) <= 0 ? v : value;
            }

            var median = Node.BranchingFactor / 2;
            var left = _indexReader.Create(current.Parent);
            Array.Copy(values, 0, left.Values, 0, median);
            var right = _indexReader.Create(current.Parent);
            Array.Copy(values, median + 1, right.Values, median, values.Length - median);
            Add(current.Parent, values[median], _indexReader.Get(current.Parent));

            return current;
        }

        public T Get(T prototype)
        {
            return Get(prototype, _root);
        }

        public T Get(T value, Node current)
        {
            int i;
            for (i = 0; i < Node.BranchingFactor; i++)
            {
                var v = _repository.Get(current.Values[i]);
                if (value.CompareTo(v) == 0) return v;
                if (value.CompareTo(v) <= 0) break;
            }

            return Get(value, _indexReader.Get(current.References[i]));
        }
    }
}