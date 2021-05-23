using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static Haengma.Core.Sgf.SgfProperty;
using Haengma.Core.Utils;

namespace Haengma.Core.Sgf
{
    public class SgfReader
    {
        private static readonly char[] Linebreaks = new[] { '\n', '\r' };
        private static readonly char[] EscapedChars = new[] { ']', '\\' };
        private static readonly char[] ComposedEscapedChars = new[] { ':', ']', '\\' };

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

        private static Parser<char, string> PropertyIdentifier => Token(char.IsUpper)
            .AtLeastOnceString()
            .Labelled("property identifier");

        private static Parser<char, IEnumerable<T>> PropertyValues<T>(Parser<char, T> parser) => parser
            .Between(Char('['), Char(']'))
            .Many()
            .Labelled("property value");

        private static Parser<char, T> PropertyValue<T>(Parser<char, T> parser) => PropertyValues(parser)
            .Assert(x => x.Count() == 1)
            .Select(x => x.Single());

        private static Parser<char, Point> Point => Token(char.IsLetter)
            .Select(c => (c - 'a') + 1)
            .Repeat(2)
            .Select(v => new Point(v.ElementAt(0), v.ElementAt(1)))
            .Labelled("Point");

        private static Parser<char, Move> Move => Try(Stone).Or(Pass);

        private static Parser<char, Move> Stone => PropertyValue(Point).Select<Move>(x => new Move.Point(x.X, x.Y));
        private static Parser<char, Move> Pass => String("[]").Select<Move>(x => new Move.Pass());

        private static Parser<char, int> Int { get; } = Token(char.IsDigit)
            .AtLeastOnce()
            .Select(ns => string.Join(string.Empty, ns))
            .Select(int.Parse);

        private static Parser<char, string> SimpleText(bool isComposed) => Whitespace
            .ThenReturn(' ')
            .Or(NormalChar(isComposed))
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Labelled("SimpleText");

        private static Parser<char, string> Text(bool isComposed) => NormalChar(isComposed)
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Labelled("Text");

        private static Parser<char, int?> Decimals => Try(Token('.').Then(Int))
            .Optional()
            .Select<int?>(x => x.HasValue ? x.Value : null);

        private static Parser<char, SgfSign> NumberSign { get; } = Char('+')
            .Or(Char('-'))
            .Select(v => v == '+' ? SgfSign.Plus : SgfSign.Minus);

        private static Parser<char, (SgfSign? sign, int number)> Number { get; } =
            (from sign in Try(NumberSign).Optional().Select<SgfSign?>(x => x.HasValue ? x.Value : null)
             from value in Int
             select (sign, value))
            .Labelled("Number");

        private static Parser<char, (SgfSign? sign, double number)> Real { get; } =
             (from number in Number
             from decimals in Decimals
             let value = number.number + (decimals?.Map(d => d / 10) ?? 0.0)
             select (number.sign, value))
            .Labelled("Real");

        private static Parser<char, (L, R)> Composed<L, R>(Parser<char, L> p1, Parser<char, R> p2) =>
            (from v1 in p1
             from separator in Char(':')
             from v2 in p2
             select (v1, v2))
            .Labelled("composed");

        private static Parser<char, SgfProperty> ToProperty(string identifier) => identifier switch
        {
            "B" => Move.Select<SgfProperty>(x => new B(x)).Labelled("B"),
            "W" => Move.Select<SgfProperty>(x => new W(x)).Labelled("W"),
            "C" => PropertyValue(Text(false)).Select<SgfProperty>(x => new C(x)).Labelled("C"),
            "PB" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new PB(x)).Labelled("PB"),
            "PW" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new PW(x)).Labelled("PW"),
            "AB" => PropertyValues(Point).Select<SgfProperty>(x => new AB(x.ToSet())).Labelled("AB"),
            "AW" => PropertyValues(Point).Select<SgfProperty>(x => new AW(x.ToSet())).Labelled("AW"),
            "SZ" => PropertyValue(Int).Select<SgfProperty>(x => new SZ(x)).Labelled("SZ"),
            "HA" => PropertyValue(Int).Select<SgfProperty>(x => new HA(x)).Labelled("HA"),
            "MN" => PropertyValue(Int).Select<SgfProperty>(x => new MN(x)).Labelled("MN"),
            "AP" => PropertyValue(Composed(SimpleText(true), SimpleText(true))).Select<SgfProperty>(x => new AP(x)).Labelled("AP"),
            "KM" => PropertyValue(Real).Select<SgfProperty>(x => new KM(x.number)).Labelled("KM"),
            "RE" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new RE(x)).Labelled("RE"),
            "BR" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new BR(x)).Labelled("BR"),
            "WR" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new WR(x)).Labelled("WR"),
            _ => PropertyValues(OneOf(Try(Text(true)), Try(Text(false)))).Select<SgfProperty>(x => new Unknown(identifier, x.ToArray()))
        };

        private static Parser<char, SgfProperty> Property =>
            from identifier in PropertyIdentifier
            from property in ToProperty(identifier).Between(SkipWhitespaces)
            select property;

        private static Parser<char, SgfNode> Node => Char(';')
            .Then(Property.Between(SkipWhitespaces).Many())
            .Select(ps => new SgfNode(ps.ToSet()))
            .Labelled("node");

        private static Parser<char, SgfGameTree> GameTree =>
            from start in Char('(')
            from sequence in Node.Between(SkipWhitespaces).Many()
            from trees in GameTree.Between(SkipWhitespaces).Many()
            from end in Char(')')
            select new SgfGameTree(sequence.ToArray(), trees.ToArray());

        public static Result<char, IReadOnlyList<SgfGameTree>> Parse(TextReader reader) => GameTree
            .Between(SkipWhitespaces)
            .Many()
            .Select<IReadOnlyList<SgfGameTree>>(ts => ts.ToArray())
            .Parse(reader);

        public static Result<char, IReadOnlyList<SgfGameTree>> Parse(string s) => new StringReader(s).Use(x => Parse(x));
    }
}
