using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfCompose : ISgfValue, IEquatable<SgfCompose>
    {
        public ISgfValue Value1 { get; }
        public ISgfValue Value2 { get; }

        public string Value => Value1.Value + ":" + Value2.Value;

        public SgfCompose(ISgfValue v1, ISgfValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfCompose);
        }

        public bool Equals(SgfCompose other)
        {
            return other != null &&
                   EqualityComparer<ISgfValue>.Default.Equals(Value1, other.Value1) &&
                   EqualityComparer<ISgfValue>.Default.Equals(Value2, other.Value2);
        }

        public override int GetHashCode()
        {
            var hashCode = -1959444751;
            hashCode = hashCode * -1521134295 + EqualityComparer<ISgfValue>.Default.GetHashCode(Value1);
            hashCode = hashCode * -1521134295 + EqualityComparer<ISgfValue>.Default.GetHashCode(Value2);
            return hashCode;
        }

        public bool Equals(ISgfValue other)
        {
            return other is SgfCompose compose && Equals(compose);
        }

        public static bool operator ==(SgfCompose left, SgfCompose right)
        {
            return EqualityComparer<SgfCompose>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfCompose left, SgfCompose right)
        {
            return !(left == right);
        }
    }
}
