using Haengma.Core.Sgf;
using Haengma.Core.Utils;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Linq;
using static Xunit.Assert;

namespace Haengma.Tests
{
    public static class SgfAssert
    {
        public static void SingleSgfTree(
            IReadOnlyList<SgfGameTree> trees,
            Action<SgfGameTree> assert = null
        )
        {
            Single(trees);
            assert?.Also(x => All(trees, x));
        }
        
        public static void ParseSuccess(
            Result<char, IReadOnlyList<SgfGameTree>> result,
            string sgf,
            Action<IReadOnlyList<SgfGameTree>> assert = null
        )
        {
            if (!result.Success)
            {
                True(false, $"Expected the SGF '{sgf}' to be valid. The message was '{result.Error.RenderErrorMessage()}'");
            }

            assert?.Invoke(result.Value);
        }

        public static void SingleNode(
            SgfGameTree tree,
            Action<SgfNode> assert = null
        )
        {
            Single(tree.Sequence);
            Empty(tree.Trees);
            assert?.Also(x => All(tree.Sequence, x));
        }

        public static void ContainsProperty<T>(SgfGameTree tree, Action<T> assert = null)
        {
            var properties = tree.Sequence.SelectMany(x => x.Properties).OfType<T>();
            NotEmpty(properties);
            assert?.Also(x => All(properties, x));
        }

        public static void ContainsSingleProperty<T>(SgfGameTree tree, Action<T> assert = null)
        {
            var properties = tree.Sequence.SelectMany(x => x.Properties).OfType<T>();
            Single(properties);
            assert?.Also(x => All(properties, x));
        }

        public static void AssertSingleSgfProperty<T>(
            Result<char, IReadOnlyList<SgfGameTree>> parseResult,
            string sgf,
            Action<T> assert = null
        ) where T : SgfProperty
        {
            ParseSuccess(parseResult, sgf, trees => {
                SingleSgfTree(trees, tree => {
                    SingleNode(tree, node =>
                    {
                        Single(node.Properties);
                        var property = node.FindProperty<T>();
                        NotNull(property);
                        assert?.Invoke(property);
                    });
                });
            });
        }
    }
}
