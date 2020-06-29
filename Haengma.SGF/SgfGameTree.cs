using System.Collections.Generic;

namespace Haengma.SGF
{
    public class SgfGameTree
    {
        public IList<SgfGameTree> GameTrees { get; set; } = new List<SgfGameTree>();
        public IList<SgfNode> Sequence { get; set; } = new List<SgfNode>();

        public override string ToString()
        {
            return $"({string.Join(string.Empty, Sequence)}{string.Join(string.Empty, GameTrees)})";
        }
    }
}
