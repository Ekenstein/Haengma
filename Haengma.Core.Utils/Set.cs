using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Core.Utils
{
    public static class Set
    {
        public static Set<T> Empty<T>() => new(Array.Empty<T>().ToHashSet());
        public static Set<T> Of<T>(params T[] values) => new(values.ToHashSet());
        public static Set<T> ToSet<T>(this IEnumerable<T> @this) => new(@this.ToHashSet());
    }

    public class Set<T> : IReadOnlySet<T>
    {
        private readonly IReadOnlySet<T> _set;

        internal Set(IReadOnlySet<T> set)
        {
            _set = set;
        }

        public int Count => _set.Count;

        public bool Contains(T item) => _set.Contains(item);

        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);

        IEnumerator IEnumerable.GetEnumerator() => _set.GetEnumerator();

        public static Set<T> operator +(Set<T> a, Set<T> b) => new(a._set.Union(b._set).ToHashSet());
        public static Set<T> operator -(Set<T> a, Set<T> b) => new(a._set.Except(b._set).ToHashSet());
    }
}
