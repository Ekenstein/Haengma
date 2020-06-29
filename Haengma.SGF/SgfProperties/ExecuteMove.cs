using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class ExecuteMove : SgfProperty
    {
        public ExecuteMove(SgfColor color, SgfPoint point) : base(color.Value, point)
        {
        }
    }
}
