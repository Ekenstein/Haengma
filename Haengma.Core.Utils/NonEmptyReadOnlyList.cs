using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Core.Utils
{
    public class NonEmptyReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _items;

        public NonEmptyReadOnlyList(T head, params T[] tail)
        {
            _items = new [] { head }.Concat(tail).ToArray();
        }

        public T this[int index] => _items[index];

        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
