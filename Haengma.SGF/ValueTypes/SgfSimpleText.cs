using Haengma.SGF.Commons;
using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfSimpleText : SgfText, IEquatable<SgfSimpleText>
    {
        public override string Value => base.Value.Replace(char.IsWhiteSpace, _ => ' ');

        public SgfSimpleText(string s, bool isComposed) : base(s, isComposed)
        {
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfSimpleText);
        }

        public bool Equals(SgfSimpleText other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public new bool Equals(ISgfValue other)
        {
            return other is SgfSimpleText text && Equals(text);
        }

        public static bool operator ==(SgfSimpleText left, SgfSimpleText right)
        {
            return EqualityComparer<SgfSimpleText>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfSimpleText left, SgfSimpleText right)
        {
            return !(left == right);
        }
    }
}
