using Monadicsh;
using Monadicsh.Extensions;
using System;
using System.IO;
using System.Text;

namespace Haengma.SGF
{
    public static class TextReaderExtensions
    {
        private const int EOF = -1;

        public static Maybe<string> ReadString(this TextReader reader, Func<char, bool> predicate)
        {
            var sb = new StringBuilder();
            Maybe<char> next;
            while ((next = reader.Read(predicate)).IsJust)
            {
                sb.Append(next.Value);
            }

            return Maybe.CreateNonEmpty(sb.ToString());
        }

        public static Maybe<char> Peek(this TextReader reader, Func<char, bool> predicate) => Maybe
            .Create(reader.Peek())
            .Guard(v => v != EOF)
            .Coalesce(v => (char)v)
            .Guard(predicate);

        public static Maybe<char> Read(this TextReader reader, Func<char, bool> predicate)
        {
            var peek = reader.Peek(predicate);
            if (!peek.Guard(predicate).IsJust)
            {
                return Maybe<char>.Nothing;
            }

            reader.Read();
            return peek;
        }

        public static void Skip(this TextReader reader, Func<char, bool> predicate)
        {
            while (reader.Read(predicate).IsJust);
        }

        public static void SkipWhitespace(this TextReader reader) => reader.Skip(char.IsWhiteSpace);
    }
}
