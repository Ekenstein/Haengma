using Haengma.SGF.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haengma.SGF.SgfProperties
{
    public class GamePlace : SgfProperty
    {
        public string? Place { get; }

        public GamePlace(string? place) : base("PC", new SgfSimpleText(place, false))
        {
            Place = place;
        }
    }
}
