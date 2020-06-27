using Haengma.SGF.ValueTypes;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Haengma.SGF.Parser
{
    public static partial class SgfParser
    {
        private static Parser<char, int> Int { get; } = Token(char.IsDigit)
            .AtLeastOnce()
            .Select(ns => string.Join(string.Empty, ns))
            .Select(int.Parse);

        private static Parser<char, NumberSign> NumberSign { get; } = Char('+')
            .Or(Char('-'))
            .Select(v => v == '+' ? ValueTypes.NumberSign.Plus : ValueTypes.NumberSign.Minus);

        public static Parser<char, SgfValue> NumberWithBoundaries(int min, int max) => Number
            .Assert(v => v is SgfNumber n && n.Number >= min && n.Number <= max, "The number was out of boundaries");

        /// <summary>
        /// [("+"|"-")] Digit { Digit }
        /// </summary>
        public static Parser<char, SgfValue> Number { get; } =
            (from sign in Try(NumberSign).Optional()
             from value in Int
             select (SgfValue)sign.Match(s => new SgfNumber(s, value), () => new SgfNumber(value)))
            .Labelled("Number");
    }
}
