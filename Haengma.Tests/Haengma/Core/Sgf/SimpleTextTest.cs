using Haengma.Core.Sgf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Xunit.Assert;
using static Haengma.Tests.RandomExtensions;

namespace Haengma.Tests.Haengma.Core.Sgf
{
    public class SimpleTextTest
    {
        [Theory]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("\r")]
        public void Ctor_String_IllegalWhiteSpaces_ArgumentException(string s)
        {
            Throws<ArgumentException>(() => new SgfSimpleText(s));
        }

        [Fact]
        public void Ctor_String_Space_SimpleText()
        {
            var instance = new SgfSimpleText(" ");
            Equal(" ", instance);
        }

        [Theory]
        [MemberData(nameof(Strings))]
        public void Ctor_RandomString_LegalChars_SimpleText(string s)
        {
            var instance = new SgfSimpleText(s);
            Equal(s, instance.Text);
        }

        [Fact]
        public void FromString_RemovesIllegalChars_SimpleText()
        {
            var result = SgfSimpleText.FromString("\rapa\n\tn");
            Equal(" apa  n", result.Text);
        }

        public static IEnumerable<object[]> Strings()
        {
            const string alphabet = UCLetters + LCLetters + Digits + SpecialChars + "     ";
            for (var i = 0; i < 100; i++)
            {
                yield return new object[] { Fixture.Rng.NextString(alphabet) };
            }
        }
    }
}
