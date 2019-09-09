﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Haengma.SGF.Commons;
using Haengma.SGF.ValueTypes;
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

        private static Parser<char, char> NormalChar { get; } = Token(c => !EscapedChars.Contains(c));

        private static Parser<char, Unit> SkipLinebreaks { get; } = Token(Linebreaks.Contains).SkipMany();

        private static Parser<char, int> Int { get; } = Token(char.IsDigit)
            .AtLeastOnce()
            .Select(ns => string.Join(string.Empty, ns))
            .Select(int.Parse);

        private static readonly Parser<char, SgfDouble> DoubleParser = Char('1')
            .Or(Char('2'))
            .Select(v => v == '1' ? SgfDouble.Normal : SgfDouble.Emphasized);

        private static readonly Parser<char, SgfColor> ColorParser = Char('B')
            .Or(Char('W'))
            .Select(v => v == 'W' ? SgfColor.White : SgfColor.Black);

        private static readonly Parser<char, SgfPoint> PointParser = Token(char.IsLetter)
            .Select(SgfPoint.CharToInt)
            .Repeat(2)
            .Select(v => new SgfPoint(v.ElementAt(0), v.ElementAt(1)));

        private static readonly Parser<char, SgfText> TextParser = NormalChar
            .Or(EscapedChar)
            .ManyString()
            .Select(v => new SgfText(v));

        private static readonly Parser<char, SgfSimpleText> SimpleTextParser = Whitespace
            .ThenReturn(' ')
            .Or(NormalChar)
            .Or(EscapedChar)
            .ManyString()
            .Select(v => new SgfSimpleText(v));

        private static readonly Parser<char, NumberSign> NumberSignParser = Char('+')
            .Or(Char('-'))
            .Select(v => v == '+' ? NumberSign.Plus : NumberSign.Minus);

        private static readonly Parser<char, SgfNumber> NumberParser =
            from sign in Try(NumberSignParser).Optional()
            from value in Int
            select new SgfNumber(sign.Select(Monadicsh.Maybe.Just).GetValueOrDefault(), value);

        private static readonly Parser<char, SgfReal> RealParser =
            from number in NumberParser
            from dot in Char('.')
            from decimals in Int
            select new SgfReal(number.Sign, number.Number + decimals / 100);

        private static Parser<char, char> EscapedChar =>
            from escape in Char('\\').Between(SkipLinebreaks)
            from c in Token(EscapedChars.Contains).Or(NormalChar)
            select c;

        private static Parser<char, ISgfValue> ComposedParser(Parser<char, ISgfValue> p1, Parser<char, ISgfValue> p2) =>
            from v1 in p1
            from separator in Char(':')
            from v2 in p2
            select (ISgfValue)new SgfCompose(v1, v2);

        /// <summary>
        /// Double values are used for annotation properties. They are called Double because the value is either simple or emphasized. A value of '1' means 'normal'; '2' means that it is emphasized.
        /// Example:
        /// GB[1] could be displayed as: Good for black.z
        /// GB[2] could be displayed as: Very good for black
        /// </summary>
        public static readonly SgfValueType Double = new SgfValueType(DoubleParser.Cast<ISgfValue>());
        public static readonly SgfValueType Color = new SgfValueType(ColorParser.Cast<ISgfValue>());
        public static readonly SgfValueType Point = new SgfValueType(PointParser.Cast<ISgfValue>());
        public static readonly SgfValueType Text = new SgfValueType(TextParser.Cast<ISgfValue>());
        public static readonly SgfValueType SimpleText = new SgfValueType(SimpleTextParser.Cast<ISgfValue>());
        public static readonly SgfValueType Real = new SgfValueType(RealParser.Cast<ISgfValue>());
        public static readonly SgfValueType Number = new SgfValueType(NumberParser.Cast<ISgfValue>());

        private SgfValueType(Parser<char, ISgfValue> parser)
        {
            Parser = parser;
        }

        internal Parser<char, ISgfValue> Parser { get; }

        public SgfValueType ComposedWith(SgfValueType type) => new SgfValueType(ComposedParser(Parser, type.Parser));
        public SgfValueType Or(SgfValueType type) => new SgfValueType(Parser.Or(type.Parser));
    }

    public class SgfReader
    {
        public class SgfReaderConfiguration
        {
            public IDictionary<UpperCaseLetterString, SgfValueType> Properties { get; } = new Dictionary<UpperCaseLetterString, SgfValueType>();
        }

        public SgfReaderConfiguration Config { get; } = new SgfReaderConfiguration();

        public Result<char, SgfCollection> Parse(TextReader reader) => Collection.Parse(reader);

        private Parser<char, UpperCaseLetterString> PropertyIdentifier => Token(char.IsUpper)
            .ManyString()
            .Assert(s => !string.IsNullOrEmpty(s))
            .Select(s => new UpperCaseLetterString(s))
            .Assert(s => Config.Properties.ContainsKey(s));

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
