using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class WhiteRank : SgfProperty
    {
        public WhiteRank(string rank) : base("WR", new SgfSimpleText(rank, false))
        {
        }
    }
}
