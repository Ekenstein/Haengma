using System;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Backend.Utils
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> block)
        {
            foreach (var item in list)
            {
                block(item);
            }
        }

        public static T? Head<T>(this IReadOnlyList<T> list) => list.Count != 0 ? list[0] : default;
        public static IReadOnlyList<T> Tail<T>(this IReadOnlyList<T> list) => list.Skip(1).ToArray();

        public static (T? head, IReadOnlyList<T> tail) HeadAndTail<T>(this IReadOnlyList<T> list) => (list.Head(), list.Tail());
    }

    public static class List
    {
        public static IReadOnlyList<T> Of<T>(params T[] values) => values ?? Empty<T>();
        public static IReadOnlyList<T> Empty<T>() => Array.Empty<T>();
        public static IReadOnlyList<T> Append<T>(this IReadOnlyList<T> @this, IReadOnlyList<T> outer) => @this.Concat(outer).ToArray();
        public static IReadOnlyList<T> RemoveRange<T>(this IReadOnlyList<T> @this, IReadOnlyList<T> outer) => @this.Except(outer).ToArray();
    }
}
