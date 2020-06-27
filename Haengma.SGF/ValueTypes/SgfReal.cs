using Pidgin;
using System.Globalization;

namespace Haengma.SGF.ValueTypes
{
    public class SgfReal : SgfValue
    {
        public Maybe<NumberSign> Sign { get; }
        public double Number { get; }

        public override string Value => Sign
            .Match(s => s == NumberSign.Minus ? "-" : "+", () => "") + Number.ToString(CultureInfo.InvariantCulture);

        public SgfReal(Maybe<NumberSign> sign, double value)  : base()
        {
            Number = value;
            Sign = sign;
        }

        public SgfReal(double value) : this(default, value) { }
    }
}
