using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haengma.SGF
{
    public class Property
    {
        public string Identifier { get; }
        public IList<string> Values { get; } = new List<string>();

        public Property(string identifier)
        {
            Identifier = identifier;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Identifier);
            foreach (var value in Values)
            {
                sb.Append('[').Append(value).Append(']');
            }

            return sb.ToString();
        }
    }
}
