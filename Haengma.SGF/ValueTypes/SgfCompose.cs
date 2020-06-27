namespace Haengma.SGF.ValueTypes
{
    public class SgfCompose : SgfValue
    {
        public SgfValue Value1 { get; }
        public SgfValue Value2 { get; }

        public override string Value => $"{Value1.Value}:{Value2.Value}";

        public SgfCompose(SgfValue v1, SgfValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
    }
}
