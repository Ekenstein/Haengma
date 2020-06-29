using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.GIB.Parser
{
    public static class GibParser
    {
        private static Parser<char, IEnumerable<GibPropertyValue>> HeaderPropertyValues => AnyCharExcept('\\', ']', ',')
            .ManyString()
            .Between(SkipWhitespaces)
            .Select(v => new GibPropertyValue(v))
            .Separated(Char(','));

        private static Parser<char, string> HeaderPropertyName => Token(char.IsUpper).AtLeastOnceString();

        private static Parser<char, KeyValuePair<string, IEnumerable<GibPropertyValue>>> HeaderProperty => 
            from start in String("\\[")
            from name in HeaderPropertyName
            from assignment in Char('=')
            from values in HeaderPropertyValues
            from end in String("\\]")
            select KeyValuePair.Create(name, values);

        private static Parser<char, Dictionary<string, IEnumerable<GibPropertyValue>>> Header => Try(HeaderProperty)
            .Between(SkipWhitespaces)
            .Many()
            .Between(String("\\HS"), String("\\HE"))
            .Select(vs => new Dictionary<string, IEnumerable<GibPropertyValue>>(vs));

        private static Parser<char, GibNode[]> GameTree => AnyCharExcept('\\', '\r', '\n')
            .ManyString()
            .Separated(Token(c => c == '\r' || c == '\n'))
            .Between(SkipWhitespaces)
            .Between(String("\\GS"), String("\\GE"))
            .Select(x => x.Where(c => !string.IsNullOrWhiteSpace(c)).Select(x => new GibNode(x)).ToArray());

        private static Parser<char, GibFile> File =>
            from header in Header.Between(SkipWhitespaces)
            from tree in GameTree.Between(SkipWhitespaces)
            select new GibFile(header, tree);

        public static Result<char, GibFile> Parse(string s) => File.Parse(s);
        public static Result<char, GibFile> Parse(TextReader s) => File.Parse(s);
    }
}
