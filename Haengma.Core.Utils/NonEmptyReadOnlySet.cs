using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Core.Utils
{
    public class NonEmptyReadOnlySet<T> : IReadOnlySet<T>
    {
        private readonly HashSet<T> _items;
        public NonEmptyReadOnlySet(T head, params T[] tail)
        {
            _items = new HashSet<T>(new [] { head }.Concat(tail));
        }

        public int Count => _items.Count;

        public bool Contains(T item) => _items.Contains(item);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<T> other) => _items.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => _items.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => _items.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => _items.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => _items.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => _items.SetEquals(other);
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
