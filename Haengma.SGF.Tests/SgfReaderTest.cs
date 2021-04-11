using Haengma.Backend.Functional.Sgf;
using Haengma.Backend.Utils;
using Xunit;
using static Haengma.Backend.Functional.Sgf.SgfProperty;

namespace Haengma.SGF.Tests
{
    public class SgfReaderTest
    {
        [Fact]
        public void EmptyTreeReturnsEmptySgfGameTree()
        {
            var sgf = "()";
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);
            Assert.All(result.Value, x =>
            {
                Assert.Empty(x.Sequence);
                Assert.Empty(x.Trees);
            });
        }

        [Fact]
        public void BlackMoveProperty()
        {
            var sgf = "(;B[aa])";
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);

            TestUtils.AssertProperty<B>(result.Value, b =>
            {
                var point = b.Move as Move.Point;
                Assert.NotNull(point);
                Assert.Equal(new Move.Point(1, 1), point);
            });

        }

        [Theory]
        [InlineData("(;B[])")]
        public void BlackMovePassProperty(string sgf)
        {
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);
            TestUtils.AssertProperty<B>(result.Value, b =>
            {
                Assert.True(b.Move is Move.Pass);
            });
        }

        [Fact]
        public void WhiteMovePointProperty()
        {
            var sgf = "(;W[aa])";
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);

            TestUtils.AssertProperty<W>(result.Value, w =>
            {
                var point = w.Move as Move.Point;
                Assert.Equal(new Move.Point(1, 1), point);
            });
        }

        [Fact]
        public void WhiteMovePassProperty()
        {
            var sgf = "(;W[])";
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);

            TestUtils.AssertProperty<W>(result.Value, w =>
            {
                Assert.True(w.Move is Move.Pass);
            });
        }

        [Fact]
        public void AddBlackStonesProperty()
        {
            var sgf = "(;AB[aa][ab][ac])";
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);
            TestUtils.AssertProperty<AB>(result.Value, ab =>
            {
                Assert.Equal(Set.Of(new Point(1, 1), new Point(1, 2), new Point(1, 3)), ab.Stones);
            });
        }

        [Theory]
        [InlineData("(;AP[CGoban:1.6.2])", "CGoban", "1.6.2")]
        public void ApplicationProperty(string sgf, string name, string version)
        {
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);
            TestUtils.AssertProperty<AP>(result.Value, ap =>
            {
                Assert.Equal(name, ap.Application.name);
                Assert.Equal(version, ap.Application.version);
            });
        }
    }
}
