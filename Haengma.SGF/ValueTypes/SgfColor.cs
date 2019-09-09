using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfColor : ISgfValue, IEquatable<SgfColor>, IEquatable<ISgfValue>
    {
        public static readonly SgfColor Black = new SgfColor("B");
        public static readonly SgfColor White = new SgfColor("W");

        private SgfColor(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfColor);
        }

        public bool Equals(SgfColor other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public bool Equals(ISgfValue other)
        {
            return other is SgfColor color && Equals(color);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public static bool operator ==(SgfColor left, SgfColor right)
        {
            return EqualityComparer<SgfColor>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfColor left, SgfColor right)
        {
            return !(left == right);
        }
    }
}
