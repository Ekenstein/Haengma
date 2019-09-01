using Monadicsh;
using Monadicsh.Extensions;
using System.IO;
using System.Linq;
using Xunit;

namespace Haengma.SGF.Tests
{
    public class SgfReaderTests
    {
        [Fact]
        public void TestParseSgf()
        {
            var value = File.ReadAllText(@"C:\temp\zhy1378-Haengma.sgf");
            var result = Parse(value);

        }

        [Fact]
        public void TestParseText()
        {
            const string value = @"(;C[Meijin NR: yeah, k4 is won\
derful
sweat NR: thank you! :\)
dada NR: yup. I like this move too. It's a move only to be expected from a pro. I really like it :)
jansteen 4d: Can anyone\
 explain [me\] k4?])";

            var result = Parse(value).GetValueOrDefault();
        }

        [Fact]
        public void TestParseEmptyTree()
        {
            const string value = "()";
            var result = Parse(value).GetValueOrDefault();
            Assert.NotNull(result);
            AssertCollection(result);
            Assert.Single(result.GameTrees);
        }

        [Fact]
        public void TestParseEmptyCollection()
        {
            const string value = "";
            var result = Parse(value).GetValueOrDefault();
            Assert.NotNull(result);
            AssertCollection(result);
            Assert.Empty(result.GameTrees);
        }

        [Fact]
        public void TestParseSingleNode()
        {
            const string sgf = "(;A[b])";
            var result = Parse(sgf).GetValueOrDefault();
            Assert.NotNull(result);
            AssertCollection(result);
            Assert.Single(result.GameTrees);
            Assert.All(result.GameTrees, gameTree =>
            {
                Assert.Single(gameTree.Sequence);
                Assert.Empty(gameTree.GameTrees);
                Assert.All(gameTree.Sequence, sequence => 
                {
                    Assert.Single(sequence.Properties);
                    Assert.All(sequence.Properties, prop =>
                    {
                        Assert.Equal("A", prop.Identifier);
                        Assert.Single(prop.Values);
                        Assert.All(prop.Values, value =>
                        {
                            Assert.Equal("b", value);
                        });
                    });
                });
            });
        }

        [Fact]
        public void TestTwoEmptyTrees()
        {
            const string sgf = "()()";
            var result = Parse(sgf).GetValueOrDefault();
            Assert.Equal(2, result.GameTrees.Count());
            Assert.All(result.GameTrees, gameTree =>
            {
                Assert.Empty(gameTree.Sequence);
                Assert.Empty(gameTree.GameTrees);
            });
        }

        private Maybe<SgfCollection> Parse(string s) => new PidginSgfReader().Parse(GetTextReader(s));

        private TextReader GetTextReader(string s) => new StringReader(s);

        private void AssertCollection(SgfCollection collection)
        {
            Assert.NotNull(collection.GameTrees);
            Assert.All(collection.GameTrees, AssertGameTree);
        }

        private void AssertGameTree(SgfGameTree gameTree)
        {
            Assert.All(gameTree.GameTrees, AssertGameTree);
            Assert.NotNull(gameTree.GameTrees);
            Assert.NotNull(gameTree.Sequence);
            Assert.All(gameTree.Sequence, AssertNode);
        }

        private void AssertNode(SgfNode node)
        {
            Assert.NotNull(node.Properties);
            Assert.All(node.Properties, AssertProperty);
        }

        private void AssertProperty(SgfProperty prop)
        {
            Assert.NotNull(prop.Values);
            Assert.NotNull(prop.Identifier);
        }
    }
}
