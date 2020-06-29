using Haengma.SGF.ValueTypes;
using System.Linq;

namespace Haengma.SGF.Extensions
{
    public static class SgfNodeExtensions
    {
        public static void AddComment(this SgfNode node, string comment)
        {
            var comments = node["C"]
                .OfType<SgfText>()
                .Select(v => v.Text)
                .Concat(new[] { comment });

            var sgf = new SgfText(string.Join("\n", comments), false);
            node["C"] = new [] { sgf };
        }

        public static string? GetComment(this SgfNode node) => node["C"]
            .OfType<SgfText>()
            .Select(v => v.Text)
            .SingleOrDefault();
    }
}
