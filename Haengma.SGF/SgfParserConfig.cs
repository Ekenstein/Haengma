using System.Collections.Generic;

namespace Haengma.SGF
{
    public class SgfParserConfig
    {
        public IList<SgfParserProperty> Properties { get; } = new List<SgfParserProperty>();
    }

    public class SgfParserProperty
    {
        public string Identifier { get; }

    }
}
