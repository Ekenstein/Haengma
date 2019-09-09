using Monadicsh;
using Monadicsh.Extensions;
using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public enum NumberSign
    {
        Plus = 0,
        Minus = 1
    }

    public class SgfNumber : ISgfValue, IEquatable<SgfNumber>
    {
        public Maybe<NumberSign> Sign { get; }
        public int Number { get; }

        public string Value
        {
            get
            {
                var sign = Sign
                    .Coalesce(v => v == NumberSign.Minus ? "-" : "+")
                    .DefaultIfNothing(string.Empty)
                    .GetValueOrDefault();

                return sign + Number.ToString();
            }
        }

        public SgfNumber(Maybe<NumberSign> sign, int value)
        {
            Sign = sign;
            Number = value;
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfNumber);
        }

        public bool Equals(SgfNumber other)
        {
            return other != null &&
                   Sign.Equals(other.Sign) &&
                   Number == other.Number;
        }

        public override int GetHashCode()
        {
            var hashCode = 577003044;
            hashCode = hashCode * -1521134295 + EqualityComparer<Maybe<NumberSign>>.Default.GetHashCode(Sign);
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            return hashCode;
        }

        public bool Equals(ISgfValue other)
        {
            return other is SgfNumber number && Equals(number);
        }

        public static bool operator ==(SgfNumber left, SgfNumber right)
        {
            return EqualityComparer<SgfNumber>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfNumber left, SgfNumber right)
        {
            return !(left == right);
        }
    }
}
