using System;

namespace Haengma.Core.Utils
{
    public static class Throwable
    {
        public static Exception Bug(string? message) => throw new InvalidOperationException(message);
    }
}
