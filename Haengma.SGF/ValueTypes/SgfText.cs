using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfText : ISgfValue, IEquatable<SgfText>
    {
        public string Value { get; }

        public SgfText(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfText);
        }

        public bool Equals(SgfText other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public bool Equals(ISgfValue other)
        {
            return other is SgfText text && Equals(text);
        }

        public static bool operator ==(SgfText left, SgfText right)
        {
            return EqualityComparer<SgfText>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfText left, SgfText right)
        {
            return !(left == right);
        }
    }
}
