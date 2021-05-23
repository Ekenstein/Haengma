using Haengma.Core.Utils;
using System;
using System.Collections.Generic;

namespace Haengma.Core.Sgf
{
    public enum SgfColor { Black, White }
    public enum SgfSign { Plus, Minus }
    
    public sealed record Point(int X, int Y);
    
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

    public sealed record SgfNode(Set<SgfProperty> Properties)
    {
        public static SgfNode Empty => new(Set.Empty<SgfProperty>());
    }

    public sealed record SgfGameTree(IReadOnlyList<SgfNode> Sequence, IReadOnlyList<SgfGameTree> Trees) 
    { 
        public static readonly SgfGameTree Empty = new(List.Empty<SgfNode>(), List.Empty<SgfGameTree>());
    }
}
