using System.Collections.Generic;
using System.Linq;

namespace Haengma.SGF
{
    /// <summary>
    /// Represents a collection of zero or more game trees.
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// A collection of zero or more <see cref="GameTree"/>.
        /// </summary>
        public IList<GameTree> GameTrees { get; } = new List<GameTree>();

        public override string ToString()
        {
            return $"{string.Join(string.Empty, GameTrees.Select(gt => $"({gt})"))}";
        }
    }
}
