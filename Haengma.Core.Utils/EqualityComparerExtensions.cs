using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Haengma.Core.Utils
{
    public static class EqualityComparerExtensions
    {
        private class Equality<T> : IEqualityComparer<T>
        {
            private readonly Func<T?, T?, bool> _equals;
            private readonly IEqualityComparer<T> _baseComparer;

            public Equality(Func<T?, T?, bool> equals, IEqualityComparer<T> baseComparer)
            {
                _equals = equals;
                _baseComparer = baseComparer;
            }

            public bool Equals(T? x, T? y) => _equals(x, y);

            public int GetHashCode([DisallowNull] T obj) => _baseComparer.GetHashCode(obj);
        }

        private class HashCode<T> : IEqualityComparer<T>
        {
            private readonly Func<T, int> _hashCode;
            private readonly IEqualityComparer<T> _baseComparer;

            public HashCode(Func<T, int> hashCode, IEqualityComparer<T> baseComparer)
            {
                _hashCode = hashCode;
                _baseComparer = baseComparer;
            }

            public bool Equals(T? x, T? y) => _baseComparer.Equals(x, y);

            public int GetHashCode([DisallowNull] T obj) => _hashCode(obj);
        }

        public static IEqualityComparer<T> WithEquals<T>(this IEqualityComparer<T> comparer, Func<T?, T?, bool> equals) => new Equality<T>(equals, comparer);
        public static IEqualityComparer<T> WithHashCode<T>(this IEqualityComparer<T> comparer, Func<T, int> hashCode) => new HashCode<T>(hashCode, comparer);
    }
}
