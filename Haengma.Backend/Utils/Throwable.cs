using System;

namespace Haengma.Backend.Utils
{
    public static class Throwable
    {
        public static Exception Bug(string? message) => throw new InvalidOperationException(message);
    }
}
