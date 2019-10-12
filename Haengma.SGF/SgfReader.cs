using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Haengma.SGF.Commons;
using Haengma.SGF.ValueTypes;
using Monadicsh;
using Monadicsh.Extensions;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.SGF
{
    public class SgfValueType
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

        private static Parser<char, SgfNone> NoneParser { get; } = SkipWhitespaces.WithResult(new SgfNone());

        private static Parser<char, char> NormalChar(bool isComposed)
        {
            var escapedChars = isComposed ? ComposedEscapedChars : EscapedChars;
            return Token(c => !escapedChars.Contains(c));
        }

        private static Parser<char, Unit> SkipLinebreaks { get; } = Token(Linebreaks.Contains).SkipMany();

        private static Parser<char, SgfList> EListOfParser(SgfValueType type) => type
            .Parser
            .Between(Token('['), Token(']'))
            .Many()
            .Select(vs => new SgfList(vs))
            .Labelled("elist of");

        private static Parser<char, SgfList> ListOfParser(SgfValueType type) => type
            .Parser
            .Between(Token('['), Token(']'))
            .AtLeastOnce()
            .Select(vs => new SgfList(vs))
            .Labelled("list of");

        private static Parser<char, ISgfValue> ComposedParser(Parser<char, ISgfValue> p1, Parser<char, ISgfValue> p2) =>
            from v1 in p1
            from separator in Char(':')
            from v2 in p2
            select (ISgfValue)new SgfCompose(v1, v2);

        private static Parser<char, int> Int { get; } = Token(char.IsDigit)
            .AtLeastOnce()
            .Select(ns => string.Join(string.Empty, ns))
            .Select(int.Parse);

        private static readonly Parser<char, SgfDouble> DoubleParser = Char('1')
            .Or(Char('2'))
            .Select(v => v == '1' ? SgfDouble.Normal : SgfDouble.Emphasized)
            .Labelled("Double");

        private static readonly Parser<char, SgfColor> ColorParser = Char('B')
            .Or(Char('W'))
            .Select(v => v == 'W' ? SgfColor.White : SgfColor.Black)
            .Labelled("Color");

        private static readonly Parser<char, SgfPoint> PointParser = Token(char.IsLetter)
            .Select(SgfPoint.CharToInt)
            .Repeat(2)
            .Select(v => new SgfPoint(v.ElementAt(0), v.ElementAt(1)))
            .Labelled("Point");

        private static Parser<char, SgfText> TextParser(bool isComposed) => NormalChar(isComposed)
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Select(v => new SgfText(v, isComposed))
            .Labelled("Text");

        private static Parser<char, SgfSimpleText> SimpleTextParser(bool isComposed) => Whitespace
            .ThenReturn(' ')
            .Or(NormalChar(isComposed))
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Select(v => new SgfSimpleText(v, isComposed))
            .Labelled("SimpleText");

        private static readonly Parser<char, NumberSign> NumberSignParser = Char('+')
            .Or(Char('-'))
            .Select(v => v == '+' ? NumberSign.Plus : NumberSign.Minus);

        private static Parser<char, SgfNumber> NumberParser((int min, int max)? boundaries = null) =>
            (from sign in Try(NumberSignParser).Optional()
             from value in Int.Assert(n => boundaries == null || (boundaries?.min <= n && boundaries?.max >= n))
             select new SgfNumber(sign.Select(Monadicsh.Maybe.Just).GetValueOrDefault(), value))
            .Labelled("Number");

        private static readonly Parser<char, SgfReal> RealParser =
            (from number in NumberParser()
             from dot in Char('.')
             from decimals in Int
             select new SgfReal(number.Sign, number.Number + decimals / 100))
            .Labelled("Real");

        private static Parser<char, char> EscapedChar(bool isComposed) =>
            from escape in Char('\\').Between(SkipLinebreaks)
            let escapeChars = isComposed ? ComposedEscapedChars : EscapedChars
            from c in Token(escapeChars.Contains).Or(NormalChar(isComposed))
            select c;

        /// <summary>
        /// Double values are used for annotation properties. They are called Double because the value is either simple or emphasized. A value of '1' means 'normal'; '2' means that it is emphasized.
        /// Example:
        /// GB[1] could be displayed as: Good for black.
        /// GB[2] could be displayed as: Very good for black
        /// </summary>
        public static readonly SgfValueType Double = new SgfValueType(DoubleParser.Cast<ISgfValue>(), "Double");
        public static readonly SgfValueType Color = new SgfValueType(ColorParser.Cast<ISgfValue>(), "Color");
        public static readonly SgfValueType Point = new SgfValueType(PointParser.Cast<ISgfValue>(), "Point");
        public static SgfValueType Text(bool isComposed) => new SgfValueType(TextParser(isComposed).Cast<ISgfValue>(), "Text");
        public static SgfValueType SimpleText(bool isComposed) => new SgfValueType(SimpleTextParser(isComposed).Cast<ISgfValue>(), "SimpleText");
        public static readonly SgfValueType Real = new SgfValueType(RealParser.Cast<ISgfValue>(), "Real");
        public static SgfValueType Number((int min, int max)? boundaries = null) => new SgfValueType(NumberParser(boundaries).Cast<ISgfValue>(), $"Number {boundaries.AsMaybe().Coalesce(b => $"({b.min} - {b.max})").DefaultIfNothing(string.Empty).GetValueOrDefault()}");
        public static SgfValueType Composed(SgfValueType v1, SgfValueType v2) => new SgfValueType(ComposedParser(v1.Parser, v2.Parser), $"{v1} : {v2}");
        public static SgfValueType Or(SgfValueType v1, SgfValueType v2) => new SgfValueType(v1.Parser.Or(v2.Parser), $"{v1} | {v2}");
        public static SgfValueType ListOf(SgfValueType v) => new SgfValueType(ListOfParser(v).Cast<ISgfValue>(), $"list of {v}");
        public static SgfValueType EListOf(SgfValueType v) => new SgfValueType(EListOfParser(v).Cast<ISgfValue>(), $"elist of {v}");
        public static readonly SgfValueType None = new SgfValueType(NoneParser.Cast<ISgfValue>(), "None");

        private readonly string _name;

        private SgfValueType(Parser<char, ISgfValue> parser, string name)
        {
            _name = name;
            Parser = parser;
        }

        public Monadicsh.Maybe<ISgfValue> Parse(string value) => Parser.Parse(value).Select(Monadicsh.Maybe.Just).GetValueOrDefault();

        internal Parser<char, ISgfValue> Parser { get; }

        public override string ToString() => _name;
    }

    public class SgfReader : ISgfReader
    {
        public class SgfReaderConfiguration
        {
            public IDictionary<UpperCaseLetterString, SgfValueType> Properties { get; } = new Dictionary<UpperCaseLetterString, SgfValueType>
            {
                { "HA", SgfValueType.Number() },
                { "KM", SgfValueType.Real },
                { "TB", SgfValueType.EListOf(SgfValueType.Point) },
                { "TW", SgfValueType.EListOf(SgfValueType.Point) },
                { "B", SgfValueType.Point },
                { "W", SgfValueType.Point },
                { "KO", SgfValueType.None },
                { "MN", SgfValueType.Number() },
                { "AB", SgfValueType.ListOf(SgfValueType.Point) },
                { "AE", SgfValueType.ListOf(SgfValueType.Point) },
                { "AW", SgfValueType.ListOf(SgfValueType.Point) },
                { "PL", SgfValueType.Color },
                { "C", SgfValueType.Text(false) },
                { "DM", SgfValueType.Double },
                { "GB", SgfValueType.Double },
                { "GW", SgfValueType.Double },
                { "HO", SgfValueType.Double },
                { "N", SgfValueType.SimpleText(false) },
                { "UC", SgfValueType.Double },
                { "V", SgfValueType.Real },
                { "BM", SgfValueType.Double },
                { "DO", SgfValueType.None },
                { "IT", SgfValueType.None },
                { "TE", SgfValueType.Double },
                { "AR", SgfValueType.ListOf(SgfValueType.Composed(SgfValueType.Point, SgfValueType.Point)) },
                { "CR", SgfValueType.ListOf(SgfValueType.Point) },
                { "DD", SgfValueType.EListOf(SgfValueType.Point) },
                { "LB", SgfValueType.ListOf(SgfValueType.Composed(SgfValueType.Point, SgfValueType.SimpleText(true))) },
                { "LN", SgfValueType.ListOf(SgfValueType.Composed(SgfValueType.Point, SgfValueType.Point)) },
                { "MA", SgfValueType.ListOf(SgfValueType.Point) },
                { "SL", SgfValueType.ListOf(SgfValueType.Point) },
                { "SQ", SgfValueType.ListOf(SgfValueType.Point) },
                { "TR", SgfValueType.ListOf(SgfValueType.Point) },
                { "AP", SgfValueType.Or(SgfValueType.Composed(SgfValueType.SimpleText(true), SgfValueType.SimpleText(true)), SgfValueType.SimpleText(false)) },
                { "CA", SgfValueType.SimpleText(false) },
                { "FF", SgfValueType.Number((1, 4)) },
                { "GM", SgfValueType.Number((1, 16)) },
                { "ST", SgfValueType.Number((0, 3)) },
                { "SZ", SgfValueType.Or(SgfValueType.Number(), SgfValueType.Composed(SgfValueType.Number(), SgfValueType.Number())) },
                { "AN", SgfValueType.SimpleText(false) },
                { "BR", SgfValueType.SimpleText(false) },
                { "BT", SgfValueType.SimpleText(false) },
                { "CP", SgfValueType.SimpleText(false) },
                { "DT", SgfValueType.SimpleText(false) },
                { "EV", SgfValueType.SimpleText(false) },
                { "GN", SgfValueType.SimpleText(false) },
                { "GC", SgfValueType.Text(false) },
                { "ON", SgfValueType.SimpleText(false) },
                { "OT", SgfValueType.SimpleText(false) },
                { "PB", SgfValueType.SimpleText(false) },
                { "PC", SgfValueType.SimpleText(false) },
                { "PW", SgfValueType.SimpleText(false) },
                { "RE", SgfValueType.SimpleText(false) },
                { "RO", SgfValueType.SimpleText(false) },
                { "RU", SgfValueType.SimpleText(false) },
                { "SO", SgfValueType.SimpleText(false) },
                { "TM", SgfValueType.Real },
                { "US", SgfValueType.SimpleText(false) },
                { "WR", SgfValueType.SimpleText(false) },
                { "WT", SgfValueType.SimpleText(false) },
                { "BL", SgfValueType.Real },
                { "OB", SgfValueType.Number() },
                { "OW", SgfValueType.Number() },
                { "WL", SgfValueType.Real },
                { "FG", SgfValueType.Or(SgfValueType.None, SgfValueType.Composed(SgfValueType.Number(), SgfValueType.SimpleText(true))) },
                { "PM", SgfValueType.Number() },
                { "VW", SgfValueType.EListOf(SgfValueType.Point) }
            };
        }

        public SgfReaderConfiguration Config { get; } = new SgfReaderConfiguration();

        public Task<Result<SgfCollection>> ReadAsync(TextReader textReader) => Task.FromResult(Collection
            .Parse(textReader)
            .Match(Result.Create, error => new Error("ParseFailure", error.RenderErrorMessage())));

        private Parser<char, UpperCaseLetterString> PropertyIdentifier => Token(char.IsUpper)
            .ManyString()
            .Assert(s => !string.IsNullOrEmpty(s), "Encountered an empty property identifier.")
            .Select(s => new UpperCaseLetterString(s))
            .Assert(s => Config.Properties.ContainsKey(s), "Unknown property identifier.");

        private Parser<char, ISgfValue> PropertyValue(UpperCaseLetterString identifier) =>
            from opening in Char('[')
            let valueParser = Config.Properties[identifier].Parser
            from value in valueParser
            from ending in Char(']')
            select value;

        private Parser<char, SgfProperty> Property =>
            from identifier in PropertyIdentifier
            from values in PropertyValue(identifier).Between(SkipWhitespaces).Many()
            select new SgfProperty(identifier, values);

        private Parser<char, SgfNode> Node =>
            from start in Char(';')
            from properties in Property.Between(SkipWhitespaces).Many()
            select new SgfNode(properties);

        private Parser<char, SgfGameTree> GameTree =>
            from start in Char('(')
            from sequence in Node.Between(SkipWhitespaces).Many()
            from gameTrees in GameTree.Between(SkipWhitespaces).Many()
            from end in Char(')')
            select new SgfGameTree(gameTrees, sequence);

        private Parser<char, SgfCollection> Collection => GameTree
            .Between(SkipWhitespaces)
            .Many()
            .Select(ts => new SgfCollection(ts));
    }
}
