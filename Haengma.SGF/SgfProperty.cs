﻿using Haengma.SGF.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Haengma.SGF
{
    public class SgfProperty
    {
        [NotNull]
        public UpperCaseLetterString Identifier { get; }
        
        [NotNull]
        public IList<SgfValue> Value { get; } = new List<SgfValue>();

        public SgfProperty(UpperCaseLetterString identifier, SgfValue value) : this(identifier, new [] { value }) { }

        public SgfProperty(UpperCaseLetterString identifier, IEnumerable<SgfValue> value)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Identifier must not be null or white space.");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Identifier = identifier;

            foreach (var v in value)
            {
                Value.Add(v);
            }
        }

        public override string ToString() => $"{Identifier}{string.Join("", Value.Select(v => $"[{v}]"))}";
    }
}
