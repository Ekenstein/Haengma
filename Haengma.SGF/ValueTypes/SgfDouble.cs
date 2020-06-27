namespace Haengma.SGF.ValueTypes
{
    public class SgfDouble : SgfValue
    {
        public static readonly SgfDouble Normal = new SgfDouble("1");
        public static readonly SgfDouble Emphasized = new SgfDouble("2");

        private SgfDouble(string v)
        {
            Value = v;
        }

        public override string Value { get; }
    }
}
