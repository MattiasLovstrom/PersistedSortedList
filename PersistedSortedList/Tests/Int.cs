using System;

namespace PersistedSortedList.Tests
{
    public class Int : IComparable, IComparable<int>
    {
        private readonly int _v;

        public Int(int v)
        {
            _v = v;
        }

        public override string ToString()
        {
            return _v.ToString();
        }

        public int CompareTo(int other)
        {
            return _v.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            Int v = (Int)obj;
            return _v.CompareTo(v._v);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Int);
        }

        public override int GetHashCode()
        {
            return _v.GetHashCode();
        }

        public bool Equals(Int other)
        {
            if (other is null) return false;
            return _v == other._v;
        }

        public static bool operator <(Int a, Int b)
        {
            return a._v < b._v;
        }

        public static bool operator >(Int a, Int b)
        {
            return a._v > b._v;
        }

        public static bool operator !=(Int a, Int b)
        {
            return !(a == b);
        }

        public static bool operator ==(Int a, Int b)
        {
            return object.Equals(a, b);
        }
    }
}