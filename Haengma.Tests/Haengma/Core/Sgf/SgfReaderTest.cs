using Haengma.Core.Sgf;
using System.Collections.Generic;
using Xunit;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;
using static Haengma.Tests.SgfAssert;
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
            AssertSingleSgfProperty<B>(result, sgf, x =>
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
            AssertSingleSgfProperty<B>(result, sgf, x =>
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
            AssertSingleSgfProperty<B>(result, sgf, x => NotNull(x.Move as Move.Pass));
        }

        [Fact]
        public void Property_W_ParsedTo_SgfProperty_W_WithPoint()
        {
            var sgf = "(;W[aa])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<W>(result, sgf, x =>
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
            AssertSingleSgfProperty<W>(result, sgf, x => NotNull(x.Move as Move.Pass));
        }

        [Fact]
        public void Property_SZ_ParsedTo_SgfProperty_SZ()
        {
            var sgf = "(;SZ[19])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<SZ>(result, sgf, x => Equal(19, x.Size));
        }

        [Fact]
        public void Property_KM_ParsedTo_SgfProperty_KM_WithDecimals()
        {
            var sgf = "(;KM[6.5])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<KM>(result, sgf, x => Equal(6.5, x.Komi));
        }

        [Fact]
        public void Property_KM_ParsedTo_SgfProperty_KM_NoDecimals()
        {
            var sgf = "(;KM[4])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<KM>(result, sgf, x => Equal(4, x.Komi));
        }

        [Fact]
        public void Property_PL_ParsedTo_SgfProperty_PL_WithColorWhite()
        {
            var sgf = "(;PL[W])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<PL>(result, sgf, x => Equal(SgfColor.White, x.Color));
        }

        [Fact]
        public void Property_PL_ParsedTo_SgfProperty_PL_WithColorBlack()
        {
            var sgf = "(;PL[B])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<PL>(result, sgf, x => Equal(SgfColor.Black, x.Color));
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
            AssertSingleSgfProperty<HA>(result, sgf, x => Equal(19, x.Handicap));
        }

        [Fact]
        public void Property_Unknown_ParsedTo_SgfProperty_Unknown()
        {
            var sgf = "(;UNKNOWN[hej])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<Unknown>(result, sgf, x =>
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
            AssertSingleSgfProperty<Unknown>(result, sgf, x =>
            {
                Equal("UNKNOWN", x.Identifier);
                Equal(ListOf(new SgfText(":")), x.Values);
            });
        }

        [Fact]
        public void Property_Unknown_ParsedTo_SgfProperty_Unknown_ComposedValueThatAreEscaped_TranslatedRaw()
        {
            var sgf = "(;UNKNOWN[apa\\:n:bepa])";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<Unknown>(result, sgf, x =>
            {
                Equal("UNKNOWN", x.Identifier);
                Equal(ListOf(new SgfText("apa:n:bepa")), x.Values);
            });
        }

        [Fact]
        public void WhiteSpaces_Ignored()
        {
            var sgf = " (   \t;   AW\t\n   [  \taa  ]\n  )";
            var result = SgfReader.Parse(sgf);
            AssertSingleSgfProperty<AW>(result, sgf, x => Equal(SetOf(new SgfPoint(1, 1)), x.Stones));
        }

        [Theory]
        [MemberData(nameof(SgfCollections))]
        public void SgfCollection_ToSgf_ParsedTo_SgfCollection(IReadOnlyList<SgfGameTree> collection)
        {
            var sgf = SgfWriter.ToSgf(collection);
            var result = SgfReader.Parse(sgf);
            ParseSuccess(result, sgf, x =>
            {
                Equal(collection, x, Fixture.CollectionComparer);
            });

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
    }
}
