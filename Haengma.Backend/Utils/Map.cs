using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Haengma.Backend.Utils
{
    public static class Map
    {
        public static Map<TKey, TValue> Of<TKey, TValue>(params (TKey, TValue)[] pairs) where TKey : notnull => new Dictionary<TKey, TValue>(pairs.Select(x => KeyValuePair.Create(x.Item1, x.Item2)))
            .Map(x => new Map<TKey, TValue>(x));

        public static Map<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull => new(new Dictionary<TKey, TValue>());

        public static Map<U, TValue> MapKeys<TKey, TValue, U>(this Map<TKey, TValue> map, Func<TKey, U> block) where TKey : notnull where U : notnull => map.Select(x => KeyValuePair.Create(block(x.Key), x.Value))
            .Map(x => new Map<U, TValue>(new Dictionary<U, TValue>(x)));

        public static Map<TKey, TValue> MapKey<TKey, TValue>(this Map<TKey, TValue> map, TKey key, Func<TValue, TValue> block) where TKey : notnull
        {
            var mutableDictionary = new Dictionary<TKey, TValue>(map)
            {
                [key] = block(map[key])
            };
            return new(mutableDictionary);
        }
    }

    public sealed class Map<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly IReadOnlyDictionary<TKey, TValue> _dic;

        internal Map(IReadOnlyDictionary<TKey, TValue> dic)
        {
            _dic = dic;
        }

        public TValue this[TKey key] => _dic[key];

        public IEnumerable<TKey> Keys => _dic.Keys;

        public IEnumerable<TValue> Values => _dic.Values;

        public int Count => _dic.Count;

        public bool ContainsKey(TKey key) => _dic.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dic.GetEnumerator();

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dic.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _dic.GetEnumerator();

        public static Map<TKey, TValue> operator +(Map<TKey, TValue> a, Map<TKey, TValue> b)
        {
            var mutableMap = new Dictionary<TKey, TValue>();

            foreach (var key in a.Keys)
            {
                mutableMap[key] = a[key];
            }

            foreach (var key in b.Keys)
            {
                mutableMap[key] = b[key];
            }

            return new(mutableMap);
        }

        public static Map<TKey, TValue> operator -(Map<TKey, TValue> a, Map<TKey, TValue> b) => new Dictionary<TKey, TValue>(a._dic.Except(b._dic))
            .Map(x => new Map<TKey, TValue>(x));

        public static Map<TKey, TValue> operator +(Map<TKey, TValue> a, (TKey, TValue) b) => b
            .Map(x => KeyValuePair.Create(b.Item1, b.Item2))
            .Map(x => new Dictionary<TKey, TValue>(a._dic.Concat(new[] { x })))
            .Map(x => new Map<TKey, TValue>(x));
    }
}
