using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.SGF.ValueTypes
{
    public class SgfList : ISgfValue, IList<ISgfValue>
    {
        private readonly IList<ISgfValue> _values;

        public SgfList()
        {
            _values = new List<ISgfValue>();
        }

        public SgfList(IEnumerable<ISgfValue> values)
        {
            _values = new List<ISgfValue>(values);
        }

        public ISgfValue this[int index] { get => _values[index]; set => _values[index] = value; }

        public string Value => string.Join("", _values.Select(v => $"[{v}]"));

        public int Count => _values.Count;

        public bool IsReadOnly => _values.IsReadOnly;

        public void Add(ISgfValue item) => _values.Add(item);

        public void Clear() => _values.Clear();

        public bool Contains(ISgfValue item) => _values.Contains(item);

        public void CopyTo(ISgfValue[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);

        public bool Equals(ISgfValue other)
        {
            return other is SgfList list && list._values.SequenceEqual(_values);
        }

        public IEnumerator<ISgfValue> GetEnumerator() => _values.GetEnumerator();

        public int IndexOf(ISgfValue item) => _values.IndexOf(item);

        public void Insert(int index, ISgfValue item) => _values.Insert(index, item);

        public bool Remove(ISgfValue item) => _values.Remove(item);

        public void RemoveAt(int index) => _values.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
