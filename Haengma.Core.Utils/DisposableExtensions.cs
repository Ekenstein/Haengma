using System;

namespace Haengma.Core.Utils
{
    public static class DisposableExtensions
    {
        public static U Use<T, U>(this T t, Func<T, U> block) where T : IDisposable
        {
            using(t)
            {
                return block(t);
            }
        }
    }
}
