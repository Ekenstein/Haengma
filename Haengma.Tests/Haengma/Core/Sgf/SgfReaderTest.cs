using Haengma.Core.Sgf;
using Pidgin;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;
using static Xunit.Assert;

namespace Haengma.Tests.Haengma.Core.Sgf
{
    public class SgfReaderTest
    {
        [Fact]
        public void Property_B_ParsedTo_SgfProperty_B_WithPoint()
        {
            var sgf = "(;B[aa])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<B>(result, sgf, x =>
            {
                var point = x.Move as Move.Point;
                NotNull(point);
                Equal(new Move.Point(1, 1), point);
            });
        }

        [Fact]
        public void Property_B_ParsedTo_SgfProperty_B_LargePoint()
        {
            var sgf = "(;B[AZ])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<B>(result, sgf, x =>
            {
                var point = x.Move as Move.Point;
                NotNull(point);
                Equal(new Move.Point(27, 52), point);
            });
        }

        [Fact]
        public void Property_B_ParsedTo_SgfProperty_B_WithPass()
        {
            var sgf = "(;B[])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<B>(result, sgf, x => NotNull(x.Move as Move.Pass));
        }

        [Fact]
        public void Property_W_ParsedTo_SgfProperty_W_WithPoint()
        {
            var sgf = "(;W[aa])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<W>(result, sgf, x =>
            {
                var point = x.Move as Move.Point;
                NotNull(point);
                Equal(new Move.Point(1, 1), point);
            });
        }

        [Fact]
        public void Property_W_ParsedTo_SgfProperty_W_WithPass()
        {
            var sgf = "(;W[])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<W>(result, sgf, x => NotNull(x.Move as Move.Pass));
        }

        [Fact]
        public void Property_SZ_ParsedTo_SgfProperty_SZ()
        {
            var sgf = "(;SZ[19])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<SZ>(result, sgf, x => Equal(19, x.Size));
        }

        [Fact]
        public void Property_KM_ParsedTo_SgfProperty_KM_WithDecimals()
        {
            var sgf = "(;KM[6.5])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<KM>(result, sgf, x => Equal(6.5, x.Komi));
        }

        [Fact]
        public void Property_KM_ParsedTo_SgfProperty_KM_NoDecimals()
        {
            var sgf = "(;KM[4])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<KM>(result, sgf, x => Equal(4, x.Komi));
        }

        [Fact]
        public void Property_PL_ParsedTo_SgfProperty_PL_WithColorWhite()
        {
            var sgf = "(;PL[W])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<PL>(result, sgf, x => Equal(SgfColor.White, x.Color));
        }

        [Fact]
        public void Property_PL_ParsedTo_SgfProperty_PL_WithColorBlack()
        {
            var sgf = "(;PL[B])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<PL>(result, sgf, x => Equal(SgfColor.Black, x.Color));
        }

        [Fact]
        public void Property_PL_ParseFails_UnknownColor()
        {
            var sgf = "(;PL[Y])";
            var result = SgfReader.Parse(sgf);
            False(result.Success, $"Expected the SGF '{sgf}' to be invalid.");
        }

        [Fact]
        public void Property_HA_ParsedTo_SgfProperty_HA()
        {
            var sgf = "(;HA[19])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<HA>(result, sgf, x => Equal(19, x.Handicap));
        }

        [Fact]
        public void Property_Unknown_ParsedTo_SgfProperty_Unknown()
        {
            var sgf = "(;UNKNOWN[hej])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<Unknown>(result, sgf, x =>
            {
                Equal("UNKNOWN", x.Identifier);
                Single(x.Values);
                All(x.Values, value => Equal("hej", value));
            });
        }

        [Fact]
        public void Property_Unknown_ParsedTo_SgfProperty_Unknown_ValueWithComposedChar()
        {
            var sgf = "(;UNKNOWN[:])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<Unknown>(result, sgf, x =>
            {
                Equal("UNKNOWN", x.Identifier);
                Equal(ListOf(":"), x.Values);
            });
        }

        [Fact]
        public void Property_Unknown_ParsedTo_SgfProperty_Unknown_ComposedValueThatAreEscaped_TranslatedRaw()
        {
            var sgf = "(;UNKNOWN[apa\\:n:bepa])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<Unknown>(result, sgf, x =>
            {
                Equal("UNKNOWN", x.Identifier);
                Equal(ListOf("apa:n:bepa"), x.Values);
            });
        }

        [Fact]
        public void ListProperties_AB_EmptyList_SgfProperty_NoValues()
        {
            var sgf = "(;AB[])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<AB>(result, sgf, x => Empty(x.Stones));
        }

        [Fact]
        public void ListProperties_AW_EmptyList_SgfProperty_NoValues()
        {
            var sgf = "(;AW[])";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<AW>(result, sgf, x => Empty(x.Stones));
        }

        [Fact]
        public void WhiteSpaces_Ignored()
        {
            var sgf = " (   \t;   AW\t\n   [  \taa  ]\n  )";
            var result = SgfReader.Parse(sgf);
            AssertSingleProperty<AW>(result, sgf, x => Equal(SetOf(new Point(1, 1)), x.Stones));
        }

        [Theory]
        [MemberData(nameof(SgfCollections))]
        public void SgfCollection_ToSgf_ParsedTo_SgfCollection(IReadOnlyList<SgfGameTree> collection)
        {
            var sgf = SgfWriter.ToSgf(collection);
            var result = SgfReader.Parse(sgf);
            if (!result.Success)
            {
                True(false, $"Failed to parse '{sgf}', message was '{result.Error.RenderErrorMessage()}'");
            }

            var actualSgf = SgfWriter.ToSgf(result.Value);
            Equal(sgf, actualSgf);
        }

        public static IEnumerable<object[]> SgfCollections()
        {
            for (var i = 0; i < 100; i++)
            {
                yield return new object[] { Fixture.NextSgfCollection() };
            }
        }

        private static void AssertSingleProperty<T>(
            Result<char, IReadOnlyList<SgfGameTree>> result,
            string sgf,
            Action<T> assert = null
        ) where T : SgfProperty
        {
            if (!result.Success)
            {
                True(false, $"Expected the SGF '{sgf}' to be valid. The message was '{result.Error.RenderErrorMessage()}'");
            }
            var collection = result.Value;
            Single(collection);
            All(collection, tree => {
                Single(tree.Sequence);
                All(tree.Sequence, node =>
                {
                    Single(node.Properties);
                    var property = node.FindProperty<T>();
                    NotNull(property);
                    assert?.Invoke(property!);
                });
            });
        }
    }
}
