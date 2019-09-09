using Haengma.SGF;
using Haengma.SGF.ValueTypes;
using System;
using System.Linq;

namespace Haengma.GS
{
    public class SgfController
    {
        public SgfGameTree NewGame() => new SgfGameTree();
        
        public void AddBlackMove(SgfGameTree gameTree, SgfPoint point) => gameTree.Sequence.Add(new SgfNode
        {
            Properties =
            {
                new SgfProperty("B")
                {
                    Values =
                    {
                        point
                    }
                }
            }
        });

        public void AddWhiteMove(SgfGameTree gameTree, SgfPoint point) => gameTree.Sequence.Add(new SgfNode
        {
            Properties =
            {
                new SgfProperty("W")
                {
                    Values = { point }
                }
            }
        });

        public void AddComment(SgfGameTree gameTree, SgfText comment)
        {
            var node = gameTree.Sequence.LastOrDefault();
            if (node == null)
            {
                node = new SgfNode();
                gameTree.Sequence.Add(node);
            }

            var property = node.Properties.SingleOrDefault(p => p.Identifier == "C");
            if (property == null)
            {
                property = new SgfProperty("C");
                node.Properties.Add(property);
            }

            var value = property.Values.SingleOrDefault();
            var newValue = string.Join("\n", new[] { value?.Value, comment.Value }.Where(v => v != null));
            property.Values.Clear();
            property.Values.Add(new SgfText(newValue));
        }
    }
}
