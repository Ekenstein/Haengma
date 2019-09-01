using Monadicsh;
using System.IO;

namespace Haengma.SGF
{
    /// <summary>
    /// Represents an abstraction of an SGF parser.
    /// </summary>
    public interface ISgfReader
    {
        Maybe<SgfCollection> Parse(TextReader reader);
    }
}
