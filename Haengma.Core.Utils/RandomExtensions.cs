using System;
using System.Collections.Generic;

namespace Haengma.Core.Utils
{
    public static class RandomExtensions
    {
        public static T Next<T>(this Random random, IReadOnlyList<T> ts)
        {
            var index = random.Next(ts.Count);
            return ts[index];
        }
    }
}
