using System;
using System.Collections.Generic;

namespace Haengma.Tests
{
    public static class RandomExtensions
    {
        public const string UCLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ";
        public const string LCLetters = "abcdefghijklmnopqrstuvwxyzåäö";
        public const string Digits = "0123456789";
        public const string SpecialChars = "!\"#¤%&/:,.;><^¨~*'()=`´@£$€{[]}\\";
        public const string WhiteSpaces = "\n\r \t";

        public static IReadOnlyList<T> NextCollection<T>(this Random random, 
            Func<Random, T> element,
            int minSize = 0, 
            int maxSize = 10)
        {
            var size = random.Next(minSize, maxSize);
            var arr = new T[size];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = element(random);
            }

            return arr;
        }

        public static T Next<T>(this Random random, IReadOnlyList<T> ts) => ts[random.Next(ts.Count)];

        public static string NextString(this Random random, 
            string alphabet = UCLetters + LCLetters + Digits + SpecialChars + WhiteSpaces,
            int minLength = 0,
            int maxLength = 20)
        {
            var charArray = alphabet.ToCharArray();
            return string.Join("", random.NextCollection(rng => rng.Next(charArray), minLength, maxLength));
        }
    }
}
