using System;
using System.IO;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.SGF
{
    public class PidginSgfReader : ISgfReader
    {
        private static readonly char[] Linebreaks = new[]
{
            '\n', '\r'
        };

        private static readonly char[] EscapedChars = new[]
        {
            ']', '\\'
        };

        private static Parser<char, char> NormalChar { get; } = Token(c => !EscapedChars.Contains(c));

        private static Parser<char, Unit> SkipLinebreaks { get; } = Token(Linebreaks.Contains).SkipMany();

        public Monadicsh.Maybe<SgfCollection> Parse(TextReader reader)
        {
            var result = Collection.Parse(reader);

            return result.GetValueOrDefault();
        }

        private static Parser<char, char> EscapedChar { get; } =
            from escape in Char('\\').Between(SkipLinebreaks)
            from c in Token(EscapedChars.Contains).Or(NormalChar)
            select c;

        private static readonly Parser<char, string> PropertyValue = 
            from opening in Char('[')
            from value in NormalChar.Or(EscapedChar).ManyString()
            from ending in Char(']')
            select value;

        private static Parser<char, SgfProperty> Property { get; } =
            from identifier in Token(char.IsUpper).ManyString().Assert(s => !string.IsNullOrEmpty(s))
            from values in PropertyValue.Between(SkipWhitespaces).Many()
            select new SgfProperty(identifier, values);

        private static Parser<char, SgfNode> Node { get; } = 
            from start in Char(';')
            from properties in Property.Between(SkipWhitespaces).Many()
            select new SgfNode(properties);

        private static Parser<char, SgfGameTree> GameTree { get; } =
            from start in Char('(')
            from sequence in Node.Between(SkipWhitespaces).Many()
            from gameTrees in GameTree.Between(SkipWhitespaces).Many()
            from end in Char(')')
            select new SgfGameTree(gameTrees, sequence);

        private static Parser<char, SgfCollection> Collection { get; } = GameTree
            .Between(SkipWhitespaces)
            .Many()
            .Select(ts => new SgfCollection(ts));
    }
}
