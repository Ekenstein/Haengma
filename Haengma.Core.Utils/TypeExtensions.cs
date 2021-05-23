using System;

namespace Haengma.Core.Utils
{
    public static class TypeExtensions
    {
        public static U Map<T, U>(this T value, Func<T, U> block) => block(value);
        public static void Apply<T>(this T value, Action<T> block) => block(value);
    }
}
