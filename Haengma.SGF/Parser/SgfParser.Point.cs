using Haengma.SGF.ValueTypes;
using Pidgin;
using System.Linq;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        public static Parser<char, SgfValue> Point => Token(char.IsLetter)
            .Select(SgfPoint.CharToInt)
            .Repeat(2)
            .Select(v => new SgfPoint(v.ElementAt(0), v.ElementAt(1)))
            .OfType<SgfValue>()
            .Labelled("Point");

        /// <summary>
        /// point | composition of point ":" point
        /// </summary>
        public static Parser<char, SgfValue> ListOfPoints => Try(Composed(Point, Point))
            .Or(Point);
    }
}
