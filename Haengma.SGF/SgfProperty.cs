using Haengma.SGF.Commons;
using Haengma.SGF.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haengma.SGF
{
    public class SgfProperty
    {
        public UpperCaseLetterString Identifier { get; }
        public IList<ISgfValue> Values { get; } = new List<ISgfValue>();

        public SgfProperty(UpperCaseLetterString identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Identifier must not be null or white space.");
            }

            Identifier = identifier;
        }

        public SgfProperty(UpperCaseLetterString identifier, IEnumerable<ISgfValue> values) : this(identifier)
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
