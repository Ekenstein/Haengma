using Monadicsh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Haengma.SGF.Commons
{
    public static class StringExtensions
    {
        public static string AppendBefore(this string s, Func<char, bool> predicate, Func<char, char> append)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            
            var sb = new StringBuilder();

            using (var reader = new StringReader(s))
            {
                do
                {
                    var next = reader.Peek(predicate);
                    if (next.IsJust)
                    {
                        sb.Append(append(next.Value));
                    }

                    sb.Append((char)reader.Read());

                } while (!reader.IsEOF());
            }

            return sb.ToString();
        }

        private static Maybe<char> Peek(this TextReader reader, Func<char, bool> predicate)
        {
            var value = reader.Peek();
            if (value == -1)
            {
                return Maybe<char>.Nothing;
            }

            var c = (char)value;
            return predicate(c)
                ? c
                : Maybe<char>.Nothing;
        }

        private static bool IsEOF(this TextReader reader) => reader.Peek() == -1;

        public static string Replace(this string s, Func<char, bool> predicate, Func<char, char> replace) 
        { 
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var sb = new StringBuilder();

            foreach (var c in s)
            {
                if (!predicate(c))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(replace(c));
                }
            }

            return sb.ToString();
        }
    }
}
