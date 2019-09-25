using Haengma.SGF.ValueTypes;
using Monadicsh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haengma.SGF.Tests
{
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random)
        {
            return random.Next(0, 2) == 1;
        }

        public static Maybe<T> NextMaybe<T>(this Random random, Func<Random, T> value)
        {
            return random.NextBool()
                ? value(random)
                : Maybe<T>.Nothing;
        }

        public static Maybe<NumberSign> NextNumberSign(this Random random)
        {
            return random.NextMaybe(rng => rng.NextBool() ? NumberSign.Minus : NumberSign.Plus);
        }

        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public const string UppercaseAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string NextString(this Random random, int min, int max, string alphabet = Alphabet)
        {
            var length = random.Next(min, max);
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                sb.Append(random.NextChar(alphabet));
            }

            return sb.ToString();
        }

        public static char NextChar(this Random random, string alphabet = Alphabet)
        {
            return alphabet[random.Next(alphabet.Length)];
        }

        public static T Next<T>(this Random random, T[] collection)
        {
            return collection[random.Next(collection.Length)];
        }
    }
}
