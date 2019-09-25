using Haengma.SGF.Commons;
using Haengma.SGF.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using static Haengma.SGF.SgfReader;

namespace Haengma.SGF.Tests
{
    public class SgfReaderTests
    {
        [Fact]
        public void TestDoubleNormal()
        {
            const string sgf = "(;A[1])";
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.Double);
            var result = Parse(instance, sgf);
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, property =>
            {
                Assert.Equal("A", property.Identifier);
                Assert.Single(property.Values);
                Assert.All(property.Values, value =>
                {
                    Assert.True(value is SgfDouble d && d.Value == "1");
                });
            });
        }

        [Fact]
        public void TestDoubleEmphasized()
        {
            const string sgf = "(;A[2])";
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.Double);
            var result = Parse(instance, sgf);
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, property =>
            {
                Assert.Equal("A", property.Identifier);
                Assert.Single(property.Values);
                Assert.All(property.Values, value =>
                {
                    Assert.True(value is SgfDouble d && d.Value == "2");
                });
            });
        }

        [Fact]
        public void TestDoubleInvalid()
        {
            const string sgf = "(;A[0])";
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.Double);
            var result = Parse(instance, sgf);
            Assert.False(result.Success);
        }

        [Fact]
        public void TestColorWhite()
        {
            var instance = new SgfReader
            { 
                Config =
                {
                    Properties = { { "A", SgfValueType.Color } }
                }
            };

            const string sgf = "(;A[W])";
            var result = Parse(instance, sgf);
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, prop =>
            {
                Assert.Single(prop.Values);
                Assert.Equal("A", prop.Identifier);
                Assert.All(prop.Values, value =>
                {
                    Assert.True(value is SgfColor color && color.Value == "W");
                });
            });
        }

        [Fact]
        public void TestColorBlack()
        {
            var instance = new SgfReader
            {
                Config =
                {
                    Properties = { { "A", SgfValueType.Color } }
                }
            };

            const string sgf = "(;A[B])";
            var result = Parse(instance, sgf);
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, prop =>
            {
                Assert.Single(prop.Values);
                Assert.Equal("A", prop.Identifier);
                Assert.All(prop.Values, value =>
                {
                    Assert.True(value is SgfColor color && color.Value == "B");
                });
            });
        }

        [Fact]
        public void TestColorInvalid()
        {
            var instance = new SgfReader
            {
                Config =
                {
                    Properties = { { "A", SgfValueType.Color } }
                }
            };

            const string sgf = "(;A[BB])";
            var result = Parse(instance, sgf);
            Assert.False(result.Success);
        }

        [Fact]
        public void TestNumber()
        {
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.Number);
            var result = Parse(instance, "(;A[+12])");
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, "A", value =>
            {
                var number = value as SgfNumber;
                Assert.NotNull(number);
                Assert.True(number.Sign.IsJust);
                Assert.Equal(NumberSign.Plus, number.Sign.Value);
                Assert.Equal(12, number.Number);
            });

            result = Parse(instance, "(;A[-12])");
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, "A", value =>
            {
                var number = value as SgfNumber;
                Assert.NotNull(number);
                Assert.True(number.Sign.IsJust);
                Assert.Equal(NumberSign.Minus, number.Sign.Value);
                Assert.Equal(12, number.Number);
            });

            result = Parse(instance, "(;A[12])");
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
            AssertValueType(result.Value, "A", value =>
            {
                var number = value as SgfNumber;
                Assert.NotNull(number);
                Assert.True(number.Sign.IsNothing);
                Assert.Equal(12, number.Number);
            });
        }

        [Fact]
        public void TestInvalidNumber()
        {
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.Number);
            var result = Parse(instance, "(;A[dwa])");
            Assert.False(result.Success);
        }

        [Fact]
        public void TestReal()
        {
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.Real);
            var result = Parse(instance, "(;A[+12.5])");
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
        }

        [Fact]
        public void TestText()
        {
            var instance = new SgfReader();
            instance.Config.Properties.Add("A", SgfValueType.SimpleText(false));
            var result = Parse(instance, "(;A[[Ekenstein\\]: Hur är det?])");
            Assert.True(result.Success);
            Assert.Single(result.Value.GameTrees);
        }

        private Pidgin.Result<char, SgfCollection> Parse(SgfReader reader, string s) => reader.Parse(new StringReader(s));

        private void AssertValueType(SgfCollection collection, UpperCaseLetterString identifier, Action<ISgfValue> assertion)
        {
            Assert.All(collection.GameTrees, gameTree =>
            {
                Assert.Single(gameTree.Sequence);
                Assert.All(gameTree.Sequence, node =>
                {
                    Assert.Single(node.Properties);
                    Assert.All(node.Properties, prop => 
                    {
                        Assert.Equal(identifier, prop.Identifier);
                        Assert.Single(prop.Values);
                        Assert.All(prop.Values, assertion);
                    });
                });
            });
        }

        private void AssertValueType(SgfCollection collection, Action<SgfProperty> assertion)
        {
            Assert.All(collection.GameTrees, gameTree =>
            {
                Assert.Single(gameTree.Sequence);
                Assert.All(gameTree.Sequence, node =>
                {
                    Assert.Single(node.Properties);
                    Assert.All(node.Properties, assertion);
                });
            });
        }

        [Theory, MemberData(nameof(Trees))]
        public void Test(SgfReader parser, SgfCollection collection)
        {
            var writer = new SgfWriter();
            var sgf = new StringBuilder();
            var textWriter = new StringWriter(sgf);
            writer.Write(textWriter, collection);

            var reader = new StringReader(sgf.ToString());
            var trees = parser.Parse(reader);
        }

        public static IEnumerable<object[]> Trees()
        {
            var random = new Random(328492);

            for (var i = 0; i < 1000; i++)
            {
                var parser = new SgfReader();
                var collection = new SgfCollection();
                var gameTree = new SgfGameTree();
                var numberOfNodes = random.Next();
                for (var j = 0; j < numberOfNodes; j++)
                {
                    var node = new SgfNode();
                    gameTree.Sequence.Add(node);
                    var property = NextProperty(random, parser.Config);
                    node.Properties.Add(property);
                }

                collection.GameTrees.Add(gameTree);
                yield return new object[] { parser, collection };
            }
        }

        private static SgfProperty NextProperty(Random random, SgfReaderConfiguration config)
        {
            UpperCaseLetterString identifier;
            do
            {
                identifier = random.NextString(1, 2, RandomExtensions.UppercaseAlphabet);
            } while (!config.Properties.ContainsKey(identifier));
            
            var value = NextValueType(random);
            config.Properties.Add(identifier, value.Item1);

            return new SgfProperty(identifier)
            {
                Values = { value.Item2 }
            };
        }

        private static (SgfValueType, ISgfValue) NextValueType(Random random)
        {
            var values = new Func<Random, (SgfValueType, ISgfValue)>[]
{
                rng => (SgfValueType.Point, new SgfPoint(rng.Next(1, 20), rng.Next(1, 20))),
                rng => (SgfValueType.Double, rng.NextBool() ? SgfDouble.Normal : SgfDouble.Emphasized),
                rng => (SgfValueType.Color, rng.NextBool() ? SgfColor.Black : SgfColor.White),
                rng => (SgfValueType.Number, new SgfNumber(random.NextNumberSign(), random.Next())),
                rng => (SgfValueType.Real, new SgfReal(random.NextNumberSign(), (decimal)random.NextDouble())),
                rng => (SgfValueType.Text(false), new SgfText(random.NextString(1, 20), false)),
                rng => (SgfValueType.SimpleText(false), new SgfSimpleText(random.NextString(1, 20), false))
            };

            return random.Next(values)(random);
        }
    }
}
