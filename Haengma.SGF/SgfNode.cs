using System.Collections.Generic;

namespace Haengma.SGF
{
    public class SgfNode
    {
        private class SgfPropertyEqualityComparer : IEqualityComparer<SgfProperty>
        {
            public bool Equals(SgfProperty? x, SgfProperty? y)
            {
                if (x == null) return false;
                if (y == null) return false;
                return x.Identifier.Equals(y.Identifier);
            }

            public int GetHashCode(SgfProperty obj)
            {
                return obj?.Identifier?.GetHashCode() ?? 0;
            }
        }



        private static readonly IEqualityComparer<SgfProperty> PropertyComparer = new SgfPropertyEqualityComparer();

        public SgfNode(IEnumerable<SgfProperty> properties)
        {
            foreach (var property in properties)
            {
                Properties.Add(property);
            }
        }

        public SgfNode() { }

        public ISet<SgfProperty> Properties { get; } = new HashSet<SgfProperty>(PropertyComparer);

        public override string ToString() => $";{string.Join(string.Empty, Properties)}";
    }
}
