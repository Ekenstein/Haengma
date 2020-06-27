using System;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.SGF.Commons
{
    public class UpperCaseLetterString : IEquatable<UpperCaseLetterString?>
    {
        public string Value { get; }

        public UpperCaseLetterString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("The string must not be null or white space.");
            }

            if (!s.All(char.IsUpper))
            {
                throw new ArgumentException("The string must be all upper case.");
            }

            Value = s;
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            return Equals(obj as UpperCaseLetterString);
        }

        public bool Equals(UpperCaseLetterString? other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static implicit operator string(UpperCaseLetterString s) => s.Value;
        public static implicit operator UpperCaseLetterString(string s) => new UpperCaseLetterString(s);

        public static bool operator ==(UpperCaseLetterString? left, UpperCaseLetterString? right)
        {
            return EqualityComparer<UpperCaseLetterString>.Default.Equals(left, right);
        }

        public static bool operator !=(UpperCaseLetterString? left, UpperCaseLetterString? right)
        {
            return !(left == right);
        }
    }
}
