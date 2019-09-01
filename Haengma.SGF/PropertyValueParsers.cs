using Monadicsh;
using Monadicsh.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Haengma.SGF
{
    public static class PropertyValueParsers
    {
        public static Maybe<string> ParseMove(TextReader reader)
        {
            var x = reader.Read(char.IsLetter);
            if (x.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            var y = reader.Read(char.IsLetter);
            if (y.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            return $"{x.Value} + {y.Value}";
        }

        public static Maybe<string> ParseColor(TextReader reader)
        {
            var color = reader.Read(c => c == 'W' || c == 'B');
            
            return color.Coalesce(v => v.ToString());
        }

        public static Maybe<string> ParseComposed(TextReader reader, Func<TextReader, Maybe<string>> p1, Func<TextReader, Maybe<string>> p2, bool loose = false)
        {
            var v1 = p1(reader);
            if (v1.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            var separator = reader.Read(c => c == ':');
            if (separator.IsNothing)
            {
                if (loose)
                {
                    return v1;
                }

                return Maybe<string>.Nothing;
            }

            var v2 = p2(reader);
            if (v2.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            return v1.Value + ":" + v2.Value;
        }

        public static Maybe<string> ParseNone(TextReader _) => string.Empty;

        public static Maybe<string> ParseDouble(TextReader reader)
        {
            var v = reader
                .Read(c => c == '1' || c == '2');
            
            return v.Coalesce(c => c.ToString());
        }

        public static Maybe<string> ParseNumber(TextReader reader, IEnumerable<int> range = null)
        {
            var sign = reader.Read(c => c == '+' || c == '-');

            var digits = reader.ReadString(char.IsDigit);
            if (digits.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            var value = int.Parse(digits.Value);
            if (sign.IsJust && sign.Value == '-')
            {
                value *= -1;
            }

            if (range != null && !range.Contains(value))
            {
                return Maybe<string>.Nothing;
            }

            return sign.Coalesce(v => v.ToString()).Or(() => string.Empty) + digits.Value;
        }

        public static Maybe<string> ParseReal(TextReader reader)
        {
            var number = ParseNumber(reader);
            if (number.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            var dot = reader.Read(c => c == '.');
            if (dot.IsNothing)
            {
                return number;
            }

            var digits = reader.ReadString(char.IsDigit);
            if (digits.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            return number.Value + "." + digits.Value;
        }

        public static Maybe<string> ParseText(TextReader reader, bool composed)
        {
            var value = new StringBuilder();
            Maybe<char> next;

            while ((next = NormalChar(reader, composed).Or(() => EscapedChar(reader, composed))).IsJust)
            {
                value.Append(next.Value);
            }

            return value.ToString();
        }

        public static Maybe<string> ParseSimpleText(TextReader reader, bool composed)
        {
            var value = new StringBuilder();
            Maybe<char> next;
            while ((next = WhitespaceExceptLinebreak(reader).Or(() => NormalChar(reader, composed)).Or(() => EscapedChar(reader, composed))).IsJust)
            {
                value.Append(next.Value);
            }

            return value.ToString();
        }

        private static bool EscapeChar(char c) => c == '\\';

        private static readonly char[] NeedsEscape = new[] { ']', '\\' };
        private static readonly char[] Linebreaks = new[] { '\n', '\r' };

        private static Maybe<char> WhitespaceExceptLinebreak(TextReader reader) => reader
            .Read(c => char.IsWhiteSpace(c) && !Linebreaks.Contains(c))
            .Coalesce(_ => ' ');

        private static Maybe<char> NormalChar(TextReader reader, bool composed)
        {
            var escapedChars = NeedsEscape
                .Concat(new[] { ':' }.Where(_ => composed));

            return reader.Read(c => !escapedChars.Contains(c));
        }

        private static Maybe<char> EscapedChar(TextReader reader, bool composed)
        {
            if (reader.Read(EscapeChar).IsNothing)
            {
                return Maybe<char>.Nothing;
            }

            reader.Skip(Linebreaks.Contains);

            var escapedChars = NeedsEscape
                .Concat(new[] { ':' }.Where(_ => composed));

            return reader
                .Read(escapedChars.Contains)
                .Or(() => NormalChar(reader, composed));
        }
    }
}
