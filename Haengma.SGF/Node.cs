using System.Collections.Generic;

namespace Haengma.SGF
{
    public class Node
    {
        public IList<Property> Properties { get; } = new List<Property>();

        public override string ToString() => $";{string.Join(string.Empty, Properties)}";
    }
}
