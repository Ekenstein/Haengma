using Pidgin;

namespace Haengma.SGF.ValueTypes
{
    public enum NumberSign
    {
        Plus = 0,
        Minus = 1
    }

    public class SgfNumber : SgfValue
    {
        public Maybe<NumberSign> Sign { get; }
        public int Number { get; }

        public override string Value => Sign.Match(s => s == NumberSign.Minus ? "-" : "+", () => "") + Number;

        public SgfNumber(NumberSign sign, int value) : this(value)
        {
            Sign = Maybe.Just(sign);
        }

        public SgfNumber(int value) 
        {
            Number = value;
        }
    }
}
