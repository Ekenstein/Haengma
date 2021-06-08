using Haengma.Core.Utils;
using System;
using System.Linq;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;

namespace Haengma.Core.Sgf
{
    public static class SgfExtensions
    {
        public static SgfColor Inverse(this SgfColor color) => color switch
        {
            SgfColor.Black => SgfColor.White,
            _ => SgfColor.Black
        };

        public static SgfNode AsNode(this SgfProperty property) => new(SetOf(property));

        public static SgfGameTree AppendNode(this SgfGameTree tree, SgfNode node) => tree with
        {
            Sequence = tree.Sequence.Append(ListOf(node))
        };

        public static SgfNode? RootNode(this SgfGameTree tree) => tree.Sequence.Head();

        public static SgfNode? LastNode(this SgfGameTree tree) => tree.Sequence.Reverse().Head();

        public static T? FindProperty<T>(this SgfNode node) => node.Properties.OfType<T>().SingleOrDefault();

        public static bool IsPass(this SgfProperty property) => property switch
        {
            B b => b.Move.IsPass,
            W w => w.Move.IsPass,
            _ => false
        };

        public static T? FindPropertyFromLastNode<T>(this SgfGameTree tree) where T : SgfProperty => tree
            .LastNode()
            ?.FindProperty<T>();

        public static SgfNode AddProperty<T>(this SgfNode node, T property) where T : SgfProperty
        {
            var oldProperties = node.Properties.OfType<T>().ToSet<SgfProperty>();
            var properties = node.Properties.Subset(oldProperties);
            return node with
            {
                Properties = properties.Merge(SetOf<SgfProperty>(property))
            };
        }

        /// <summary>
        /// Appends the given property to the last node of the tree. If there are no nodes in the
        /// tree, a new node containing the given property will be added to the tree.
        /// </summary>
        public static SgfGameTree AddPropertyToLastNode<T>(this SgfGameTree tree, T property) where T : SgfProperty
        {
            var lastNode = tree.LastNode()?.AddProperty(property) ?? property.AsNode();
            return tree with
            {
                Sequence = tree.Sequence.Take(tree.Sequence.Count - 1).Append(lastNode).ToArray()
            };
        }

        public static SgfGameTree AddRootProperty<T>(this SgfGameTree tree, T property) where T : SgfProperty
        {
            var rootNode = tree.RootNode()?.AddProperty(property) ?? property.AsNode();

            return tree with
            {
                Sequence = ListOf(rootNode).Append(tree.Sequence.Tail().ToArray())
            };
        }
    }
}
