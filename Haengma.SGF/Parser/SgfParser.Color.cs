using Pidgin;
using static Pidgin.Parser;
using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        /// <summary>
        /// ("B" | "W")
        /// </summary>
        public static Parser<char, SgfValue> Color { get; } = Char('B')
            .Or(Char('W'))
            .Select(v => v == 'W' ? SgfColor.White : SgfColor.Black)
            .OfType<SgfValue>()
            .Labelled("Color");
    }
}
