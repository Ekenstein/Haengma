using Haengma.Core.Sgf;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Haengma.SGF.Tests
{
    public static class TestUtils
    {
        public static void AssertProperty<T>(IReadOnlyList<SgfGameTree> collection, Action<T> assert) where T : SgfProperty
        {
            var properties = collection.SelectMany(x => x.Sequence).SelectMany(x => x.Properties).OfType<T>();
            Assert.NotEmpty(properties);
            Assert.All(properties, p =>
            {
                assert(p);
            });

        }

        public static U AssertAndGetPropertyValue<T, U>(IReadOnlyList<SgfGameTree> collection, Func<T, U> block)
        {
            var property = collection
                .SelectMany(x => x.Sequence)
                .SelectMany(x => x.Properties)
                .OfType<T>()
                .SingleOrDefault();

            Assert.NotNull(property);
            return block(property);
        }
    }
}
