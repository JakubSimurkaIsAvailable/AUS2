
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUS2
{
    public class NumberData : IData, IComparable
    {
        public int Value { get; set; }

        public NumberData(int value)
        {
            Value = value;
        }

        public int Compare(IData otherData)
        {
            if (otherData is NumberData other)
            {
                if (Value < other.Value) return -1;
                if (Value > other.Value) return 1;
                return 0;
            }
            throw new ArgumentException("Invalid data type for comparison.");
        }

        public override string ToString() => Value.ToString();

        public override bool Equals(object obj)
        {
            return obj is NumberData other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(object? obj)
        {
            if (obj is NumberData other)
            {
                if (Value < other.Value) return -1;
                if (Value > other.Value) return 1;
                return 0;
            }

            throw new ArgumentException("Object is not a NumberData");
        }

        public IData toData(string line)
        {
            throw new NotImplementedException();
        }
    }
}
