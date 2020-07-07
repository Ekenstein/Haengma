using Haengma.SGF.Commons;
using Pidgin;
using System.Collections.Generic;
using System.Linq;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using PropertyParsers = System.Collections.Generic.IDictionary<Haengma.SGF.Commons.UpperCaseLetterString, Pidgin.Parser<char, Haengma.SGF.SgfValue>>;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        public static Result<char, SgfCollection> Parse(string s, PropertyParsers properties) => Collection(properties).Parse(s);

        private static Parser<char, UpperCaseLetterString> PropertyIdentifier => Token(char.IsUpper)
            .ManyString()
            .Assert(s => !string.IsNullOrEmpty(s), "Encountered an empty property identifier.")
            .Select(s => new UpperCaseLetterString(s));

        private static Parser<char, IEnumerable<SgfValue>> PropertyValue(Parser<char, SgfValue> valueParser) => valueParser
            .Between(Char('['), Char(']'))
            .Between(SkipWhitespaces)
            .Many();

        private static Parser<char, SgfProperty> Property(PropertyParsers parsers) => 
            from identifier in PropertyIdentifier.Assert(parsers.ContainsKey)
            let valueParser = parsers[identifier]
            from value in PropertyValue(valueParser).Between(SkipWhitespaces)
            select new SgfProperty(identifier, value);

        private static Parser<char, SgfNode> Node(PropertyParsers parsers) =>
            from start in Char(';')
            from properties in Property(parsers).Between(SkipWhitespaces).Many()
            select new SgfNode(properties);

        private static Parser<char, SgfGameTree> GameTree(PropertyParsers properties) =>
            from start in Char('(')
            from sequence in Node(properties).Between(SkipWhitespaces).Many()
            from gameTrees in GameTree(properties).Between(SkipWhitespaces).Many()
            from end in Char(')')
            select new SgfGameTree(sequence, gameTrees);

        private static Parser<char, SgfCollection> Collection(PropertyParsers properties) => GameTree(properties)
            .Between(SkipWhitespaces)
            .Many()
            .Select(ts => new SgfCollection(ts));
    }
}
