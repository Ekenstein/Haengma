using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Haengma.Core.Utils
{
    public static class Collections
    {
        public static NonEmptyReadOnlyList<T> NonEmptyListOf<T>(T head, params T[] tail) => new(head, tail);
        public static NonEmptyReadOnlySet<T> NonEmptySetOf<T>(T head, params T[] tail) => new(head, tail);
        public static NonEmptyReadOnlySet<T> ToNonEmptySet<T>(this IEnumerable<T> ts) => new(ts.First(), ts.Tail().ToArray());
        public static NonEmptyReadOnlyList<T> ToNonEmptyList<T>(this IEnumerable<T> ts) => new(ts.First(), ts.Tail().ToArray());
        public static IReadOnlyList<T> ListOf<T>(params T[] ts) => ts;
        public static IReadOnlyList<T> EmptyList<T>() => Array.Empty<T>();

        public static IReadOnlySet<T> SetOf<T>(params T[] ts) => new ReadOnlySet<T>(new HashSet<T>(ts));
        public static IReadOnlySet<T> EmptySet<T>() => new ReadOnlySet<T>(new HashSet<T>(capacity: 0));

        public static IReadOnlySet<T> ToSet<T>(this IEnumerable<T> ts) => new ReadOnlySet<T>(ts.ToHashSet());

        public static IReadOnlyDictionary<K, V> ToReadOnly<K, V>(this IDictionary<K, V> dic) where K : notnull => new ReadOnlyDictionary<K, V>(dic);

        public static IReadOnlyDictionary<K, V> EmptyMap<K, V>() where K : notnull => new ReadOnlyDictionary<K, V>(new Dictionary<K, V>(capacity: 0));
        public static IReadOnlyDictionary<K, V> MapOf<K, V>(params (K, V)[] pairs) where K : notnull => new Dictionary<K, V>(pairs.Select(ToKvp))
            .Let(x => new ReadOnlyDictionary<K, V>(x));

        public static IDictionary<K, V> MutableMapOf<K, V>(params (K, V)[] pairs) where K : notnull => new Dictionary<K, V>(pairs.Select(ToKvp).ToArray());
        public static IDictionary<K, V> EmptyMutableMap<K, V>() where K : notnull => new Dictionary<K, V>();

        private static KeyValuePair<K, V> ToKvp<K, V>((K, V) pair) => new(pair.Item1, pair.Item2);

        public static IReadOnlyDictionary<U, V> MapKeys<K, V, U>(this IReadOnlyDictionary<K, V> dic, Func<K, U> block) where U : notnull => MapOf(
            dic.Select(x => block(x.Key).To(x.Value)).ToArray()
        );

        public static IReadOnlyDictionary<K, V> MapKey<K, V>(this IReadOnlyDictionary<K, V> dic, K key, Func<V, V> block) where K : notnull => dic[key]
            .Let(x => dic.Except(key).Add(key.To(block(x))));

        public static IReadOnlyDictionary<K, U> MapValues<K, V, U>(this IReadOnlyDictionary<K, V> dic, Func<KeyValuePair<K, V>, U> block) where K : notnull => MapOf(
            dic.Select(x => x.Key.To(block(x))).ToArray()
        );

        public static IReadOnlyDictionary<K, V> Merge<K, V>(this IReadOnlyDictionary<K, V> innerDic, IReadOnlyDictionary<K, V> outerDic) where K : notnull
        {
            var dic = new Dictionary<K, V>(innerDic);
            foreach (var kvp in outerDic)
            {
                dic[kvp.Key] = kvp.Value;
            }

            return dic.ToReadOnly();
        }

        public static IReadOnlyDictionary<K, V> Except<K, V>(this IReadOnlyDictionary<K, V> innerDic, IReadOnlyList<K> keys) where K : notnull => MapOf(
            innerDic.Where(x => !keys.Contains(x.Key)).Select(x => x.Key.To(x.Value)).ToArray()
        );

        public static IReadOnlyDictionary<K, V> Except<K, V>(this IReadOnlyDictionary<K, V> dic, K key) where K : notnull => dic.Except(new [] { key });

        public static IReadOnlyDictionary<K, V> Add<K, V>(this IReadOnlyDictionary<K, V> dic, (K, V) pair) where K : notnull => dic.Merge(MapOf(pair));

        public static IReadOnlySet<T> Merge<T>(this IReadOnlySet<T> innerSet, IReadOnlySet<T> outerSet) => 
            SetOf(innerSet.Union(outerSet).ToArray());

        public static IReadOnlySet<T> Subset<T>(this IReadOnlySet<T> innerSet, IReadOnlySet<T> outerSet) =>
            SetOf(innerSet.Except(outerSet).ToArray());

        private class ReadOnlySet<T> : IReadOnlySet<T>
        {
            private readonly ISet<T> _set;
            public ReadOnlySet(ISet<T> set)
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
        }

        private class ReadOnlyDictionary<K, V> : IReadOnlyDictionary<K, V> where K : notnull
        {
            private readonly IDictionary<K, V> _dic;

            public ReadOnlyDictionary(IDictionary<K, V> dic)
            {
                _dic = dic;
            }

            public V this[K key] => _dic[key];

            public IEnumerable<K> Keys => _dic.Keys;

            public IEnumerable<V> Values => _dic.Values;

            public int Count => _dic.Count;

            public bool ContainsKey(K key) => _dic.ContainsKey(key);

            public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => _dic.GetEnumerator();

            public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value) => _dic.TryGetValue(key, out value);

            IEnumerator IEnumerable.GetEnumerator() => _dic.GetEnumerator();
        }
    }
}
