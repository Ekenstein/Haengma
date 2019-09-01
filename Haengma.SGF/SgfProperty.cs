using System;
using System.Collections.Generic;
using System.Text;

namespace Haengma.SGF
{
    public class SgfProperty
    {
        public string Identifier { get; }
        public IList<string> Values { get; } = new List<string>();

        public SgfProperty(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Identifier must not be null or white space.");
            }

            Identifier = identifier;
        }

        public SgfProperty(string identifier, IEnumerable<string> values) : this(identifier)
        {
            foreach (var value in values)
            {
                Values.Add(value);
            }
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
