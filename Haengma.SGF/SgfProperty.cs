using Haengma.SGF.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Haengma.SGF
{
    public class SgfProperty : List<SgfValue>
    {
        [NotNull]
        public UpperCaseLetterString Identifier { get; }

        public SgfProperty(UpperCaseLetterString identifier) : this(identifier, new SgfValue[0])
        {
        }

        public SgfProperty(UpperCaseLetterString identifier, SgfValue value) : this(identifier, new [] { value }) { }

        public SgfProperty(UpperCaseLetterString identifier, IEnumerable<SgfValue> value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Identifier must not be null or white space.");
            }

            Identifier = identifier;
        }

        public override string ToString() => $"{Identifier}{string.Join("", this.Select(v => $"[{v}]").DefaultIfEmpty("[]"))}";
    }
}
