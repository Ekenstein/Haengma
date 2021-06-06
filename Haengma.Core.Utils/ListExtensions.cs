using System;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Core.Utils
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

        public static T? Head<T>(this IEnumerable<T> list) => list.FirstOrDefault();
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> list) => list.Skip(1);

        public static (T? head, IEnumerable<T> tail) HeadAndTail<T>(this IEnumerable<T> list) => (list.Head(), list.Tail());

        public static IEnumerable<U> SelectNotNull<T, U>(this IEnumerable<T> ts, Func<T, U?> select) => ts.Select(select).Where(x => x != null).Select(x => x!);
        public static T SingleOrThrow<T>(this IEnumerable<T> ts, Func<Exception> block)
        {
            var result = ts.Take(2).ToArray();
            if (result.Length <= 0 || result.Length > 1)
            {
                throw block();
            }

            return result[0];
        }

        public static IReadOnlyList<T> Subsequence<T>(this IReadOnlyList<T> ts, int startIndex, int length) => ts.Skip(startIndex).Take(length).ToArray();
        public static bool IsSingle<T>(this IReadOnlyList<T> ts) => ts.Count == 1;

        public static string JoinToString<T>(this IEnumerable<T> ts, 
            string separator, 
            Func<T, string> block
        ) => string.Join(separator, ts.Select(block));

        public static string JoinToString<T>(this IEnumerable<T> ts, string separator) => ts
            .JoinToString(separator, x => x?.ToString() ?? string.Empty);
    }

    public static class List
    {
        public static IReadOnlyList<T> Of<T>(params T[] values) => values ?? Empty<T>();
        public static IReadOnlyList<T> Empty<T>() => Array.Empty<T>();
        public static IReadOnlyList<T> Append<T>(this IReadOnlyList<T> @this, IReadOnlyList<T> outer) => @this.Concat(outer).ToArray();
        public static IReadOnlyList<T> RemoveRange<T>(this IReadOnlyList<T> @this, IReadOnlyList<T> outer) => @this.Except(outer).ToArray();
    }
}
