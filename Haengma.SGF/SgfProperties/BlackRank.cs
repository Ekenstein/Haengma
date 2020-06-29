using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class BlackRank : SgfProperty
    {
        public BlackRank(string rank) : base("BR", new SgfSimpleText(rank, false)) { }
    }
}
