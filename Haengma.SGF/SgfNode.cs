using Haengma.SGF.Commons;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<SgfValue> this[UpperCaseLetterString identifier]
        {
            get => Properties.Where(x => x.Identifier == identifier).SelectMany(x => x.Value);
            set
            {
                var existingProperty = Properties.SingleOrDefault(x => x.Identifier == identifier);
                if (existingProperty != null)
                {
                    Properties.Remove(existingProperty);
                }

                Properties.Add(new SgfProperty(identifier, value));
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
