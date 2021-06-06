using Haengma.Core.Sgf;
using Xunit;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;
using static Xunit.Assert;

namespace Haengma.Tests.Haengma.Core.Sgf
{
    public class SgfWriterTest
    {
        [Fact]
        public void SimpleText_BackSlash_Escaped()
        {
            var tree = SgfGameTree.Empty.AppendPropertyToLastNode(new PB("\\"));
            var sgf = SgfWriter.ToSgf(tree);
            Equal(@"(;PB[\\])", sgf);

            var parsedTree = SgfReader.Parse(sgf);
            True(parsedTree.Success);
            Equal(ListOf(tree), parsedTree.Value, Fixture.CollectionComparer);
        }

        [Fact]
        public void Unknown_Values_EscapeChars()
        {
            var tree = SgfGameTree.Empty.AppendPropertyToLastNode(
                new Unknown("A", ListOf(@"\"))
            );

            var sgf = SgfWriter.ToSgf(tree);
            Equal(@"(;A[\\])", sgf);
        }

        [Fact]
        public void Unknown_Values_ComposedChar_TranslatedWithoutEscape()
        {
            var tree = SgfGameTree.Empty.AppendPropertyToLastNode(
                new Unknown("A", ListOf(":"))
            );

            var sgf = SgfWriter.ToSgf(tree);
            Equal("(;A[:])", sgf);
        }
    }
}
