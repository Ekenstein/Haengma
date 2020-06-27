using System;
using System.Linq;
using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        private static readonly char[] Linebreaks = new[]
        {
            '\n', '\r'
        };

        private static readonly char[] EscapedChars = new[]
        {
            ']', '\\'
        };

        private static readonly char[] ComposedEscapedChars = new[] { ':' }
            .Concat(EscapedChars)
            .ToArray();

        private static Parser<char, Unit> SkipLinebreaks { get; } = Token(Linebreaks.Contains).SkipMany();

        private static Parser<char, char> NormalChar(bool isComposed)
        {
            var escapedChars = isComposed ? ComposedEscapedChars : EscapedChars;
            return Token(c => !escapedChars.Contains(c));
        }

        private static Parser<char, char> EscapedChar(bool isComposed) =>
            from escape in Char('\\').Between(SkipLinebreaks)
            let escapeChars = isComposed ? ComposedEscapedChars : EscapedChars
            from c in Token(escapeChars.Contains).Or(NormalChar(isComposed))
            select c;

        public static Parser<char, SgfValue> Text(bool isComposed) => NormalChar(isComposed)
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Select(v => new SgfText(v, isComposed))
            .OfType<SgfValue>()
            .Labelled("Text");
    }
}
