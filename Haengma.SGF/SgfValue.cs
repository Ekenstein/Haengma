using System;
using System.Collections.Generic;

namespace Haengma.SGF
{
    public abstract class SgfValue : IEquatable<SgfValue?>
    {
        public abstract string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            return Equals(obj as SgfValue);
        }

        public bool Equals(SgfValue? other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(SgfValue? left, SgfValue? right)
        {
            return EqualityComparer<SgfValue>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfValue? left, SgfValue? right)
        {
            return !(left == right);
        }
    }
}
