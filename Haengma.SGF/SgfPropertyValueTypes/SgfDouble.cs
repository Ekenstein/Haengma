using System;
using System.Collections.Generic;

namespace Haengma.SGF.SgfPropertyValueTypes
{
    public sealed class SgfDouble : IEquatable<SgfDouble>
    {
        private readonly string _value;

        public static readonly SgfDouble Normal = new SgfDouble("1");
        public static readonly SgfDouble Emphasized = new SgfDouble("2");

        private SgfDouble(string value) 
        { 
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SgfDouble);
        }

        public bool Equals(SgfDouble other)
        {
            return other != null &&
                   _value == other._value;
        }

        public override int GetHashCode()
        {
            return -1939223833 + EqualityComparer<string>.Default.GetHashCode(_value);
        }
    }
}
