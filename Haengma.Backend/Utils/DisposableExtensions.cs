using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haengma.Backend.Utils
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
