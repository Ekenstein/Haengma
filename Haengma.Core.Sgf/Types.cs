using Haengma.Core.Utils;
using System;
using System.Collections.Generic;
using static Haengma.Core.Utils.Collections;
using static Haengma.Core.Utils.Assertion;
using System.Linq;

namespace Haengma.Core.Sgf
{
    public enum SgfColor { Black, White }
    public enum SgfSign { Plus, Minus }

    public enum SgfEmote { Greetings, Bye, Impressed, Thanks, Mistake }

    public sealed record Point(int X, int Y);

    public class SgfText : IEquatable<SgfText?>
    {
        public string Text { get; }
        public SgfText(string s)
        {
            Require(!ContainsIllegalChars(s), $"{s} contains illegal white spaces.");
            Text = s;
        }

        public override string ToString() => Text;

        private static readonly char[] LegalWhitespaces = new [] { ' ', '\n', '\r' };
        private static bool ContainsIllegalChars(string s) => s.Any(IsIllegalChar);

        private static bool IsIllegalChar(char c) => char.IsWhiteSpace(c) && !LegalWhitespaces.Contains(c);

        public static SgfText FromString(string s) => s
            .Select(x => IsIllegalChar(x) ? ' ' : x)
            .JoinToString("")
            .Let(x => new SgfText(x));

        public override bool Equals(object? obj)
        {
            return Equals(obj as SgfText);
        }

        public bool Equals(SgfText? other)
        {
            return other != null &&
                   Text == other.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }

        public static implicit operator string(SgfText text) => text.Text;

        public static bool operator ==(SgfText? left, SgfText? right)
        {
            return EqualityComparer<SgfText>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfText? left, SgfText? right)
        {
            return !(left == right);
        }

        public SgfText AppendLine(SgfText text) => new(Text + Environment.NewLine + text.Text);
    }

    /// <summary>
    /// Represents the SimpleText in the SGF format.
    /// SimpleText doesn't contain any other white spaces except space. 
    /// </summary>
    public class SgfSimpleText : IEquatable<SgfSimpleText?>
    {
        public string Text { get; }

        public SgfSimpleText(string s)
        {
            Require(!ContainsIllegalChars(s), $"{s} contains illegal white spaces.");
            Text = s;
        }

        public static implicit operator string(SgfSimpleText text) => text.Text;

        public static bool operator ==(SgfSimpleText? left, SgfSimpleText? right)
        {
            return EqualityComparer<SgfSimpleText>.Default.Equals(left, right);
        }

        public static bool operator !=(SgfSimpleText? left, SgfSimpleText? right)
        {
            return !(left == right);
        }

        public override string ToString() => Text;

        /// <summary>
        /// Creates an instance of SimpleText from the given string.
        /// All white spaces will be replaced with spaces.
        /// </summary>
        public static SgfSimpleText FromString(string s) => s
            .Select(x => IsIllegalChar(x) ? ' ' : x)
            .JoinToString("")
            .Let(x => new SgfSimpleText(x));

        private static bool ContainsIllegalChars(string s) => s.Any(IsIllegalChar);
        private static bool IsIllegalChar(char c) => char.IsWhiteSpace(c) && c != ' ';

        public override bool Equals(object? obj)
        {
            return Equals(obj as SgfSimpleText);
        }

        public bool Equals(SgfSimpleText? other)
        {
            return other != null &&
                   Text == other.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }
    }

    public abstract record Move
    {
        public sealed record Point(int X, int Y) : Move;
        public sealed record Pass() : Move;

        public bool IsPass => this is Pass;
    }

    public record Stone(SgfColor Color, Point Point);

    internal static class MoveExtensions
    {
        internal static Point? ToPoint(this Move move) => move switch
        {
            Move.Pass => null,
            Move.Point p => new Point(p.X, p.Y),
            _ => throw new ArgumentException("Invalid move")
        };
    }

    public sealed record SgfNode(IReadOnlySet<SgfProperty> Properties)
    {
        public static SgfNode Empty => new(EmptySet<SgfProperty>());
    }

    public sealed record SgfGameTree(IReadOnlyList<SgfNode> Sequence, IReadOnlyList<SgfGameTree> Trees)
    {
        public static readonly SgfGameTree Empty = new(List.Empty<SgfNode>(), List.Empty<SgfGameTree>());
    }
}
