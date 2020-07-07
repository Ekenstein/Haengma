using System.Collections.Generic;

namespace Haengma.SGF
{
    public class SgfGameTree : List<SgfNode>
    {
        public IList<SgfGameTree> GameTrees { get; } = new List<SgfGameTree>();

        public SgfGameTree()
        {
        }

        public SgfGameTree(IEnumerable<SgfNode> sequence) : base(sequence)
        {
        }

        public SgfGameTree(IEnumerable<SgfNode> sequence, IEnumerable<SgfGameTree> trees) : this(sequence)
        {
            foreach (var tree in trees)
            {
                GameTrees.Add(tree);
            }
        }

        public override string ToString()
        {
            return $"({string.Join(string.Empty, this)}{string.Join(string.Empty, GameTrees)})";
        }
    }
}
