using Haengma.SGF.Commons;

namespace Haengma.SGF.ValueTypes
{
    public class SgfSimpleText : SgfText
    {
        public SgfSimpleText(string s, bool isComposed) : base(s, isComposed)
        {
        }

        public override string Value => base
            .Value
            .Replace(char.IsWhiteSpace, _ => ' ');
    }
}
