using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class OvertimeDescription : SgfProperty
    {
        public static OvertimeDescription ByoYomi(int periods, int timePerPeriod) => new OvertimeDescription($"{periods}x{timePerPeriod} byo-yomi");

        public OvertimeDescription(string description) : base("OT", new SgfSimpleText(description, false))
        {
        }
    }
}
