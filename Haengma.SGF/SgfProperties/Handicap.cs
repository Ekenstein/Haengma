using Haengma.SGF.ValueTypes;
using System;

namespace Haengma.SGF.SgfProperties
{
    public class Handicap : SgfProperty
    {
        public int NumberOfStones { get; }

        public Handicap(int handicap) : base("HA", new SgfNumber(handicap))
        {
            if (handicap < 2)
            {
                throw new ArgumentException("Handicap must be larger or equal to 2.");
            }

            NumberOfStones = handicap;
        }
    }
}
