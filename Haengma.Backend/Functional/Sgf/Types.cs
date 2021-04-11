using Haengma.Backend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Backend.Functional.Sgf
{
    public enum Color { Black, White }
    public enum Sign { Plus, Minus }
    public record Point(int X, int Y);
    public abstract record Move
    {
        public record Point(int X, int Y) : Move;
        public record Pass() : Move;
    }

    public record Stone(Color Color, Point Point);

    internal static class MoveExtensions
    {
        internal static Point? ToPoint(this Move move) => move switch
        {
            Move.Pass => null,
            Move.Point p => new Point(p.X, p.Y),
            _ => throw new ArgumentException("Invalid move")
        };
    }

    public static class SgfHelpers
    {
        public static Color Inverse(this Color color) => color switch
        {
            Color.Black => Color.White,
            _ => Color.Black
        };

        public static SgfNode AsNode(this SgfProperty property) => new (Set.Of(property));

        public static SgfGameTree AppendNode(this SgfGameTree tree, SgfNode node) => tree with
        {
            Sequence = tree.Sequence.Append(List.Of(node))
        };

        public static SgfGameTree PrependNode(this SgfGameTree tree, SgfNode node) => tree with
        {
            Sequence = List.Of(node).Append(tree.Sequence)
        };

        public static SgfNode? RootNode(this SgfGameTree tree) => tree.Sequence.Head();

        public static T? FindProperty<T>(this SgfNode node) => node.Properties.OfType<T>().SingleOrDefault();

        public static SgfNode AddProperty<T>(this SgfNode node, T property) where T : SgfProperty
        {
            var oldProperties = node.Properties.OfType<T>().ToSet<SgfProperty>();
            var properties = node.Properties - oldProperties;
            return node with
            {
                    Properties = properties + Set.Of<SgfProperty>(property)
            };
        }

        public static SgfGameTree AddRootProperty<T>(this SgfGameTree tree, T property) where T : SgfProperty
        {
            var rootNode = tree.RootNode()?.AddProperty(property) ?? property.AsNode();

            return tree with
            {
                Sequence = List.Of(rootNode).Append(tree.Sequence.Tail())
            };
        }
    }

    public sealed record SgfNode(Set<SgfProperty> Properties);
    public sealed record SgfGameTree(IReadOnlyList<SgfNode> Sequence, IReadOnlyList<SgfGameTree> Trees) 
    { 
        public static readonly SgfGameTree Empty = new(List.Empty<SgfNode>(), List.Empty<SgfGameTree>());
    }
}
