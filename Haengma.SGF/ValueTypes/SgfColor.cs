namespace Haengma.SGF.ValueTypes
{
    public class SgfColor : SgfValue
    {
        public static readonly SgfColor Black = new SgfColor("B");
        public static readonly SgfColor White = new SgfColor("W");

        private SgfColor(string value)
        {
            Value = value;
        }

        public override string Value { get; }
    }
}
