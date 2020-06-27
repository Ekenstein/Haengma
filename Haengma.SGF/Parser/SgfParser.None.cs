using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        public static Parser<char, SgfValue> None { get; } = SkipWhitespaces
            .WithResult(new SgfNone())
            .OfType<SgfValue>();
    }
}
