using Monadicsh;
using System;
using System.Collections.Generic;

namespace Haengma.SGF
{
    public class GameTree
    {
        public IList<GameTree> GameTrees { get; } = new List<GameTree>();
        public IList<Node> Sequence { get; } = new List<Node>();

        public override string ToString()
        {
            return $"({string.Join(string.Empty, Sequence)}{string.Join(string.Empty, GameTrees)})";
        }
    }
}
