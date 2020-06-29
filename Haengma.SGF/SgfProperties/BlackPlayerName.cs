using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class BlackPlayerName : SgfProperty
    {
        public string Name { get; }

        public BlackPlayerName(string name) : base("PB", new SgfSimpleText(name, false))
        {
            Name = name;
        }
    }
}
