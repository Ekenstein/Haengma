using Haengma.Core;
using shortid;
using shortid.Configuration;

namespace Haengma.GS
{
    public class ShortIdGenerator : IIdGenerator<string>
    {
        private readonly GenerationOptions _options;

        public ShortIdGenerator(GenerationOptions options)
        {
            _options = options;
        }

        public string Generate() => ShortId.Generate(_options);
    }
}
