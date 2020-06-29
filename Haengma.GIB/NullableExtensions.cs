using System;

namespace Haengma.GIB
{
    public static class NullableExtensions
    {
        public static U Select<T, U>(this T? value, Func<T, U> map, U defaultValue) where T : struct
        {
            if (value.HasValue)
            {
                return map(value.Value);
            }

            return defaultValue;
        }

        public static T? Where<T>(this T? value, Func<T, bool> predicate) where T : struct
        {
            if (value.HasValue && predicate(value.Value))
            {
                return value;
            }

            return default;
        }
    }
}
