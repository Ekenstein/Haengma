using Haengma.SGF.ValueTypes;
using System;

namespace Haengma.SGF.SgfProperties
{
    public class GameDate : SgfProperty
    {
        public DateTime Date { get; }

        public GameDate(DateTime date) : base("DT", new SgfSimpleText(date.ToString("yyyy-MM-dd"), false))
        {
            Date = date;
        }
    }
}
