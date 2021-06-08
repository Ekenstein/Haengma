using Haengma.Core.Utils;
using Pidgin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.Core.Sgf
{
    public static class SgfReader
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

        private static Parser<char, IEnumerable<T>> PropertyValues<T>(Parser<char, T> parser) => 
            parser.Between(Char('['), Char(']')).Many().Labelled("property value");

        private static Parser<char, T> PropertyValue<T>(Parser<char, T> parser) => PropertyValues(parser)
            .Assert(x => x.Count() == 1)
            .Select(x => x.Single());

        private static Parser<char, SgfPoint> Point => Token(char.IsLetter)
            .Between(SkipWhitespaces)
            .Assert(x => x >= 'a' && x <= 'z' || x >= 'A' && x <= 'Z')
            .Select(c => {
                if (c >= 'a' && c <= 'z')
                {
                    return c - 'a' + 1;
                }
                else
                {
                    return c - 'A' + 27;
                }
            })
            .Repeat(2)
            .Select(v => new SgfPoint(v.ElementAt(0), v.ElementAt(1)))
            .Labelled("Point");

        private static Parser<char, Move> Move => Try(Stone).Or(Pass);

        private static Parser<char, Move> Stone => PropertyValue(Point).Select<Move>(x => new Move.Point(x.X, x.Y));
        private static Parser<char, Move> Pass => String("[]").Select<Move>(x => new Move.Pass());

        private static Parser<char, int> Int { get; } = Token(char.IsDigit)
            .AtLeastOnce()
            .Select(ns => string.Join(string.Empty, ns))
            .Select(int.Parse);

        private static Parser<char, SgfSimpleText> SimpleText(bool isComposed) => Whitespace
            .ThenReturn(' ')
            .Or(NormalChar(isComposed))
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Select(x => new SgfSimpleText(x))
            .Labelled("SimpleText");

        private static readonly char[] LegalWhitespacesInText = new [] { '\n', ' ', '\r' };

        private static Parser<char, SgfText> Text(bool isComposed) => Token(x => char.IsWhiteSpace(x) && !LegalWhitespacesInText.Contains(x))
            .ThenReturn(' ')
            .Or(NormalChar(isComposed))
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Select(x => new SgfText(x))
            .Labelled("Text");

        private static Parser<char, SgfColor> Color => OneOf(Char('B'), Char('W')).Select(x => x switch
        {
            'B' => SgfColor.Black,
            'W' => SgfColor.White,
            _ => throw new InvalidOperationException($"Bug: Expected one of 'W' and 'B' but got {x}.")
        });

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
            "AB" => PropertyValues(Point).Select<SgfProperty>(x => new AB(x.ToNonEmptySet())).Labelled("AB"),
            "AW" => PropertyValues(Point).Select<SgfProperty>(x => new AW(x.ToNonEmptySet())).Labelled("AW"),
            "SZ" => PropertyValue(Int).Select<SgfProperty>(x => new SZ(x)).Labelled("SZ"),
            "HA" => PropertyValue(Int).Select<SgfProperty>(x => new HA(x)).Labelled("HA"),
            "MN" => PropertyValue(Int).Select<SgfProperty>(x => new MN(x)).Labelled("MN"),
            "AP" => PropertyValue(Composed(SimpleText(true), SimpleText(true))).Select<SgfProperty>(x => new AP(x.Item1, x.Item2)).Labelled("AP"),
            "KM" => PropertyValue(Real).Select<SgfProperty>(x => new KM(x)).Labelled("KM"),
            "RE" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new RE(x)).Labelled("RE"),
            "BR" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new BR(x)).Labelled("BR"),
            "WR" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new WR(x)).Labelled("WR"),
            "PL" => PropertyValue(Color).Select<SgfProperty>(x => new PL(x)).Labelled("PL"),
            "EM" => PropertyValue(Composed(Color, Int)).Select<SgfProperty>(x => new EM(x.Item1, (SgfEmote)x.Item2)).Labelled("Emote"),
            "OT" => PropertyValue(SimpleText(false)).Select<SgfProperty>(x => new OT(x)).Labelled("OT"),
            _ => PropertyValues(Text(false)).Select<SgfProperty>(x => new Unknown(identifier, x.ToNonEmptyList()))
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

        private static Parser<char, IReadOnlyList<SgfGameTree>> Collection => GameTree
            .Between(SkipWhitespaces)
            .Many()
            .Select(x => ListOf(x.ToArray()));

        public static Result<char, IReadOnlyList<SgfGameTree>> Parse(TextReader reader) => Collection
            .Parse(reader);

        public static Result<char, IReadOnlyList<SgfGameTree>> Parse(string s) => new StringReader(s).Use(x => Parse(x));
    }
}
