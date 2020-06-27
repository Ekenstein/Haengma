using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        public static Parser<char, SgfValue> SimpleText(bool isComposed) => Whitespace
            .ThenReturn(' ')
            .Or(NormalChar(isComposed))
            .Or(EscapedChar(isComposed))
            .ManyString()
            .Select(v => new SgfSimpleText(v, isComposed))
            .OfType<SgfValue>()
            .Labelled("SimpleText");
    }
}
