using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class WhitePlayerName : SgfProperty
    {
        public string Name { get; }

        public WhitePlayerName(string name) : base("PW", new SgfSimpleText(name, false))
        {
            Name = name;
        }
    }
}
