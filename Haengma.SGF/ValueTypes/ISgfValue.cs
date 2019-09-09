using System;

namespace Haengma.SGF.ValueTypes
{
    public interface ISgfValue : IEquatable<ISgfValue>
    {
        string Value { get; }
    }
}
