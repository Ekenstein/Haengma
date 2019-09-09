using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfDouble : ISgfValue, IEquatable<SgfDouble>
    {
        public static readonly SgfDouble Normal = new SgfDouble("1");
        public static readonly SgfDouble Emphasized = new SgfDouble("2");

        private SgfDouble(string v)
        {
            Value = v;
        }

        public string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfDouble);
        }

        public bool Equals(SgfDouble other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public bool Equals(ISgfValue other)
        {
            return other is SgfDouble d && Equals(d);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public static bool operator ==(SgfDouble left, SgfDouble right)
        {
            return EqualityComparer<SgfDouble>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfDouble left, SgfDouble right)
        {
            return !(left == right);
        }
    }
}
