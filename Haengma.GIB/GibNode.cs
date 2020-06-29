using System;

namespace Haengma.GIB
{
    public class GibNode
    {
        public GibNode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value must not be null or white space.");
            }

            Values = value.Split(" ");
        }

        public string[] Values { get; }

        public override string ToString() => string.Join(" ", Values);
    }
}
