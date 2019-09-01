using System.Collections.Generic;

namespace Haengma.SGF
{
    public class SgfGameTree
    {
        public IList<SgfGameTree> GameTrees { get; } = new List<SgfGameTree>();
        public IList<SgfNode> Sequence { get; } = new List<SgfNode>();

        public SgfGameTree(IEnumerable<SgfGameTree> trees, IEnumerable<SgfNode> sequence)
        {
            foreach (var tree in trees)
            {
                GameTrees.Add(tree);
            }

            foreach (var node in sequence)
            {
                Sequence.Add(node);
            }
        }

        public SgfGameTree() { }

        public override string ToString()
        {
            return $"({string.Join(string.Empty, Sequence)}{string.Join(string.Empty, GameTrees)})";
        }
    }
}
