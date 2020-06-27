using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        /// <summary>
        /// ("1" | "2")
        /// </summary>
        public static Parser<char, SgfValue> Double { get; } = Char('1')
            .Or(Char('2'))
            .Select(v => v == '1' ? SgfDouble.Normal : SgfDouble.Emphasized)
            .OfType<SgfValue>()
            .Labelled("Double");
    }
}
