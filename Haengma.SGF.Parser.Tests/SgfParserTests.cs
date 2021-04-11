using Haengma.SGF.Commons;
using Pidgin;
using System.IO;
using Xunit;

namespace Haengma.SGF.Parser.Tests
{
    public class SgfParserTests
    {
        [Fact]
        public void EmptyTree()
        {
            var parser = new SgfReader();
            var result = Parse(parser, "()");
            Assert.True(result.Success);
            var collection = result.GetValueOrDefault();
            Assert.NotNull(collection);
            Assert.Single(collection.Trees);
            Assert.All(collection.Trees, tree => 
            {
                Assert.Empty(tree.Sequence);
                Assert.Empty(tree.Trees);
            });
        }

        [Theory]
        [InlineData("test", "test")]
        [InlineData(@"Arnold [1k\]: hello!", "Arnold [1k]: hello!")]
        [InlineData(@"Meijin NR: yeah, k4 is won\
derful
sweat NR: thank you! :\)
dada NR: yup. I like this move too. It's a move only to be expected from a pro. I really like it :)
jansteen 4d: Can anyone\
 explain [me\] k4?"
            , @"Meijin NR: yeah, k4 is wonderful
sweat NR: thank you! :)
dada NR: yup. I like this move too. It's a move only to be expected from a pro. I really like it :)
jansteen 4d: Can anyone explain [me] k4?")]
        public void Text(string input, string expectedOutput)
        {
            var sgf = @$"(;C[{input}])";
            var parser = new SgfReader();
            var result = Parse(parser, sgf);
            Assert.True(result.Success);
            var collection = result.GetValueOrDefault();
            Assert.NotNull(collection);
            Assert.Single(collection.Trees);
            Assert.All(collection.Trees, tree =>
            {
                Assert.Single(tree.Sequence);
                Assert.Empty(tree.Trees);
                Assert.All(tree.Sequence, node =>
                {
                    Assert.Single(node.Properties);
                    Assert.All(node.Properties, property =>
                    {
                        Assert.Equal("C", property.Name);
                        Assert.Single(property.Values);
                        Assert.All(property.Values, value => Assert.Equal(expectedOutput, value));
                    });
                });
            });
        }

        public Result<char, SgfCollection> Parse(SgfReader reader, string s) => reader.Parse(new StringReader(s));
    }
}
