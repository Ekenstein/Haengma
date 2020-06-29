using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class Komi : SgfProperty
    {
        public Komi(double komi) : base("KM", new SgfReal(komi))
        {
        }
    }
}
