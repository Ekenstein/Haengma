using System.Collections.Generic;
using System.Linq;

namespace Haengma.SGF
{
    /// <summary>
    /// Represents a collection of zero or more game trees.
    /// </summary>
    public class SgfCollection
    {
        /// <summary>
        /// A collection of zero or more <see cref="SgfGameTree"/>.
        /// </summary>
        public IList<SgfGameTree> GameTrees { get; } = new List<SgfGameTree>();

        public SgfCollection() { }

        public SgfCollection(IEnumerable<SgfGameTree> trees)
        {
            foreach (var tree in trees)
            {
                GameTrees.Add(tree);
            }
        }

        public override string ToString()
        {
            return $"{string.Join(string.Empty, GameTrees.Select(gt => $"({gt})"))}";
        }
    }
}
