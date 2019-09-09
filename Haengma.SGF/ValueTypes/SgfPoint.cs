using System;
using System.Collections.Generic;

namespace Haengma.SGF.ValueTypes
{
    public class SgfPoint : ISgfValue, IEquatable<SgfPoint>
    {
        public int X { get; }
        public int Y { get; }

        public string Value => ToSgfPoint(X, Y);

        public SgfPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        private static string ToSgfPoint(int x, int y)
        {
            return $"{IntToChar(x)}{IntToChar(y)}";
        }

        private static char IntToChar(int x) => (char)(x + 'a');
        public static int CharToInt(char c) => c - 'a';

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfPoint);
        }

        public bool Equals(SgfPoint other)
        {
            return other != null &&
                   X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public bool Equals(ISgfValue other)
        {
            return other is SgfPoint point && Equals(point);
        }

        public static bool operator ==(SgfPoint left, SgfPoint right)
        {
            return EqualityComparer<SgfPoint>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfPoint left, SgfPoint right)
        {
            return !(left == right);
        }
    }
}
