using System.Collections.Generic;
using System.Linq;

namespace Haengma.SGF
{
    /// <summary>
    /// Represents a collection of zero or more game trees.
    /// </summary>
    public class SgfCollection : List<SgfGameTree>
    {

        public SgfCollection() { }

        public SgfCollection(IEnumerable<SgfGameTree> trees) : base(trees)
        {
        }

        public override string ToString()
        {
            return $"{string.Join(string.Empty, this.Select(gt => $"({gt})"))}";
        }
    }
}
