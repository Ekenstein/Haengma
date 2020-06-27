using System;
using System.Collections.Generic;

namespace GTP
{
    public class Color : IEquatable<Color?>
    {
        private readonly string _color;

        public static readonly Color Black = new Color("black");
        public static readonly Color White = new Color("white");

        private Color(string color)
        {
            _color = color;
        }

        public override string ToString() => _color;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Color);
        }

        public bool Equals(Color? other)
        {
            return other != null &&
                   _color == other._color;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_color);
        }

        public static bool operator ==(Color? left, Color? right)
        {
            return EqualityComparer<Color>.Default.Equals(left, right);
        }

        public static bool operator !=(Color? left, Color? right)
        {
            return !(left == right);
        }
    }
}
