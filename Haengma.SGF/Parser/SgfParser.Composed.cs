using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        /// <summary>
        /// ValueType ":" ValueType
        /// </summary>
        public static Parser<char, SgfValue> Composed(Parser<char, SgfValue> p1, Parser<char, SgfValue> p2) =>
            (from v1 in p1
            from separator in Char(':')
            from v2 in p2
            select (SgfValue)new SgfCompose(v1, v2))
            .Labelled("composed");
    }
}
