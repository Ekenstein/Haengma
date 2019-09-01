using System.IO;

namespace Haengma.SGF
{
    public interface ISgfWriter
    {
        void Write(TextWriter writer, SgfCollection collection);
    }
}
