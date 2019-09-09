using Monadicsh;
using Monadicsh.Extensions;
using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfReal : ISgfValue, IEquatable<SgfReal>
    {
        public Maybe<NumberSign> Sign { get; }
        public decimal Number { get; }

        public string Value { get; }

        public SgfReal(Maybe<NumberSign> sign, decimal value) 
        {
            Number = value;
            Sign = sign;
        }

        public override string ToString() => Sign
            .Coalesce(v => v == NumberSign.Minus ? "-" : "+")
            .DefaultIfNothing(string.Empty)
            .GetValueOrDefault() + Number.ToString();

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfReal);
        }

        public bool Equals(SgfReal other)
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
            return other is SgfReader real && Equals(real);
        }

        public static bool operator ==(SgfReal left, SgfReal right)
        {
            return EqualityComparer<SgfReal>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfReal left, SgfReal right)
        {
            return !(left == right);
        }
    }
}
