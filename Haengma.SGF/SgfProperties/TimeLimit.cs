using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class TimeLimit : SgfProperty
    {
        public TimeLimit(double timelimit) : base("TM", new SgfReal(timelimit))
        {
        }
    }
}
