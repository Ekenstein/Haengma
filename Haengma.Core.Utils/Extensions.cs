using System;

namespace Haengma.Core.Utils
{
    public static class Extensions
    {
        public static (T, U) To<T, U>(this T first, U second) => (first, second);
        public static U Let<T, U>(this T t, Func<T, U> block) => block(t);
        public static T Also<T>(this T t, Action<T> block) => t.Let(o =>
        {
            block(o);
            return o;
        });

        public static U Use<T, U>(this T t, Func<T, U> block) where T : IDisposable
        {
            using (t)
            {
                return block(t);
            }
        }
    }
}
