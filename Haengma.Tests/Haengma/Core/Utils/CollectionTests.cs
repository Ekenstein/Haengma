using Haengma.Core.Utils;
using System;
using System.Collections.Generic;
using Xunit;
using static Haengma.Core.Utils.Collections;
using static Xunit.Assert;

namespace Haengma.Tests.Haengma.Core.Utils
{
    public class CollectionTests
    {
        [Fact]
        public void ListOf_NoElements_EmptyList()
        {
            Equal(ListOf<string>(), EmptyList<string>());
        }

        [Fact]
        public void ListOf_Elements_ListOfElements()
        {
            var list = ListOf(1, 2, 3);
            Equal(3, list.Count);
            Contains(1, list);
            Contains(2, list);
            Contains(3, list);
        }

        [Fact]
        public void EmptyList_NoElements()
        {
            Empty(EmptyList<string>());
        }

        [Fact]
        public void MapOf_NoPairs_EmptyMap()
        {
            Equal(MapOf<int, string>(), EmptyMap<int, string>());
        }

        [Fact]
        public void EmptyMap_NoKeyValues()
        {
            Empty(EmptyMap<int, string>());
        }

        [Fact]
        public void EmptyMap_NotCastableToMutableMap()
        {
            AssertNotCastable(EmptyMap<int, string>());
        }

        [Fact]
        public void MapOf_Pairs_DictionaryWithElements()
        {
            var map = MapOf(1.To("Test"), 2.To("Test2"));
            Equal(2, map.Count);
            Equal("Test", map[1]);
            Equal("Test2", map[2]);
        }

        [Fact]
        public void MapOf_NoElements_NotCastableToMutableMap()
        {
            AssertNotCastable(MapOf<int, string>());
        }

        [Fact]
        public void MapOf_Elements_NotCastableToMutableMap()
        {
            AssertNotCastable(MapOf(1.To("Test")));
        }

        [Fact]
        public void MutableMapOf_NoPairs_EmptyMutableMap()
        {
            Equal(MutableMapOf<int, string>(), EmptyMutableMap<int, string>());
        }

        [Fact]
        public void EmptyMutableMap_DictionaryWithNoElements()
        {
            Empty(EmptyMutableMap<int, string>());
        }

        [Fact]
        public void ToReadOnly_MutableMap_CanNotBeCastedToMutable()
        {
            AssertNotCastable(EmptyMutableMap<int, string>().ToReadOnly());
        }

        [Fact]
        public void Merge_EmptyMap_ElementsOfInner()
        {
            var map = MapOf(1.To("Test"));
            var mergedMap = map.Merge(EmptyMap<int, string>());
            Equal(map, mergedMap);
        }

        [Fact]
        public void Merge_NonEmptyMap_ElementsOfBothMaps()
        {
            var inner = MapOf(1.To("Test"));
            var outer = MapOf(2.To("Test2"));

            var mergedMap = inner.Merge(outer);

            Equal(2, mergedMap.Count);
            Contains(1, mergedMap.Keys);
            Contains(2, mergedMap.Keys);
            Equal("Test", mergedMap[1]);
            Equal("Test2", mergedMap[2]);
        }

        [Fact]
        public void Merge_ResultNotCastableToMutableMap()
        {
            var inner = MapOf(1.To("Test"));
            var outer = MapOf(2.To("Test2"));

            AssertNotCastable(inner.Merge(outer));
        }

        [Fact]
        public void Merge_ConflictingMap_ValuesFromOuterMap()
        {
            var inner = MapOf(1.To("Test"));
            var outer = MapOf(1.To("Test2"));

            var mergedMap = inner.Merge(outer);
            Equal(1, mergedMap.Count);
            Contains("Test2", mergedMap[1]);
        }

        [Fact]
        public void MapKey_NoKey_ThrowsKeyNotFoundException()
        {
            Throws<KeyNotFoundException>(() => MapOf(1.To("Test")).MapKey(2, x => x));
        }

        [Fact]
        public void MapKey_KeyFound_ValueCorrespondsToKey()
        {
            MapOf(1.To("Test")).MapKey(1, x =>
            {
                Equal("Test", x);
                return x;
            });
        }

        [Fact]
        public void MapKey_KeyFound_ResultEqualsBlockResult()
        {
            var map = MapOf(1.To("Test")).MapKey(1, x => "Test2");
            Equal(1, map.Count);
            Equal("Test2", map[1]);
        }

        [Fact]
        public void MapKey_ResultNotCastableToMutableMap()
        {
            AssertNotCastable(MapOf(1.To("Test")).MapKey(1, x => x));
        }

        private static void AssertNotCastable<K, V>(IReadOnlyDictionary<K, V> dic) => Throws<InvalidCastException>(() => (IDictionary<K, V>)dic);
    }
}
