using Haengma.SGF.Parser;
using Haengma.SGF.ValueTypes;
using Pidgin;
using Xunit;

namespace Haengma.SGF.Tests
{
    public class SgfParserTests
    {
        [Theory]
        [InlineData("B")]
        [InlineData("W")]
        public void Color_Valid(string s)
        {
            var result = SgfParser.Color.Parse(s);
            Assert.True(result.Success);

            var color = result.Value as SgfColor;
            Assert.NotNull(color);

            Assert.True(SgfParser.Color.Parse(color.Value).Success);
        }

        [Theory]
        [InlineData("w")]
        [InlineData("b")]
        [InlineData("")]
        [InlineData(" ")]
        public void Color_Invalid(string s)
        {
            var result = SgfParser.Color.Parse(s);
            Assert.False(result.Success);
        }

        
    }
}
