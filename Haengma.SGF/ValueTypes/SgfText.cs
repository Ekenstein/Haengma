using System;
using System.Collections.Generic;
using System.Linq;
using Haengma.SGF.Commons;

namespace Haengma.SGF.ValueTypes
{
    public class SgfText : ISgfValue, IEquatable<SgfText>
    {
        private static readonly char[] NeedsEscape = new [] { ']', '\\' };

        private static readonly char[] NeedsEscapeWhenComposed = new [] { ':' }.Concat(NeedsEscape).ToArray();

        private const char EscapeChar = '\\';

        public string Text { get; }
        public bool IsComposed { get; }

        public virtual string Value => Text
            .Replace(c => char.IsWhiteSpace(c) && c != '\n' && c != '\r', _ => ' ')
            .AppendBefore(c => IsComposed 
                ? NeedsEscapeWhenComposed.Contains(c) 
                : NeedsEscape.Contains(c), 
            _ => EscapeChar);

        public SgfText(string value, bool isComposed)
        {
            Text = value;
            IsComposed = isComposed;
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
