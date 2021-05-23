using Haengma.Core.Utils;
using System;
using System.Linq;
using static Haengma.Core.Sgf.SgfProperty;

namespace Haengma.Core.Sgf
{
    public static class SgfHelpers
    {
        public static SgfColor Inverse(this SgfColor color) => color switch
        {
            SgfColor.Black => SgfColor.White,
            _ => SgfColor.Black
        };

        public static SgfNode AsNode(this SgfProperty property) => new(Set.Of(property));

        public static SgfGameTree AppendNode(this SgfGameTree tree, SgfNode node) => tree with
        {
            Sequence = tree.Sequence.Append(List.Of(node))
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

        /// <summary>
        /// Appends the given property to the last node of the tree. If there are no nodes in the
        /// tree, a new node containing the given property will be added to the tree.
        /// </summary>
        public static SgfGameTree AppendPropertyToLastNode<T>(this SgfGameTree tree, T property) where T : SgfProperty
        {
            var lastNode = tree.LastNode()?.AddProperty(property) ?? property.AsNode();
            return tree with
            {
                Sequence = tree.Sequence.Take(tree.Sequence.Count - 1).Append(lastNode).ToArray()
            };
        }

        public static T? FindPropertyFromLastNode<T>(this SgfGameTree tree) where T : SgfProperty => tree
            .LastNode()
            ?.FindProperty<T>();

        public static SgfGameTree AddOrUpdatePropertyOnLastNode<T>(this SgfGameTree tree, Func<T, T> onUpdate, Func<T> onAdd) where T : SgfProperty
        {
            var property = tree.FindPropertyFromLastNode<T>()?.Map(onUpdate) ?? onAdd();

            return tree.AppendPropertyToLastNode(property);
        }

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
                Sequence = List.Of(rootNode).Append(tree.Sequence.Tail().ToArray())
            };
        }
    }
}
