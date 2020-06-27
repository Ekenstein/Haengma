using System;
using System.Linq;
using Haengma.SGF.Commons;

namespace Haengma.SGF.ValueTypes
{
    public class SgfText : SgfValue
    {
        private static readonly char[] NeedsEscape = new [] { ']', '\\' };

        private static readonly char[] NeedsEscapeWhenComposed = new [] { ':' }.Concat(NeedsEscape).ToArray();

        private const char EscapeChar = '\\';

        public string Text { get; }
        public bool IsComposed { get; }

        public override string Value => Text
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
    }
}
