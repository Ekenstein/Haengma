using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class Size : SgfProperty
    {
        public Size(int size) : base("SZ", new SgfNumber(size))
        {
        }

        public Size(int width, int height) : base("SZ", new SgfCompose(new SgfNumber(width), new SgfNumber(height)))
        {
        }
    }
}
