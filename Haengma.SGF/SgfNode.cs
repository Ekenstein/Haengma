using System.Collections.Generic;

namespace Haengma.SGF
{
    public class SgfNode
    {
        public SgfNode(IEnumerable<SgfProperty> properties)
        {
            foreach (var property in properties)
            {
                Properties.Add(property);
            }
        }

        public SgfNode() { }

        public IList<SgfProperty> Properties { get; } = new List<SgfProperty>();

        public override string ToString() => $";{string.Join(string.Empty, Properties)}";
    }
}
