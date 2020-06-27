using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        private static Parser<char, Maybe<int>> Decimals { get; } = Try(Token('.').Then(Int)).Optional();

        /// <summary>
        /// Number ["." Digit { Digit }]
        /// </summary>
        public static Parser<char, SgfValue> Real { get; } =
            (from sign in Try(NumberSign).Optional()
             from number in Int
             from decimals in Decimals
             let value = number + (decimals.Select(d => d / 10).GetValueOrDefault())
             select (SgfValue)new SgfReal(sign, value))
            .Labelled("Real");
    }
}
