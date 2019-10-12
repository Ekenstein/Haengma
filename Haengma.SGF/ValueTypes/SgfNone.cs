namespace Haengma.SGF.ValueTypes
{
    public struct SgfNone : ISgfValue
    {
        public string Value => string.Empty;

        public bool Equals(ISgfValue other)
        {
            return other is SgfNone;
        }
    }
}
