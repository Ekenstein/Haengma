using Haengma.SGF.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.SGF
{
    public class SgfController
    {

        public string? GetComment(SgfNode node) => node["C"]
            .OfType<SgfText>()
            .Select(v => v.Text)
            .SingleOrDefault();

        public void AddComment(SgfNode node, string comment)
        {
            var comments = node["C"]
                .OfType<SgfText>()
                .Select(v => v.Text)
                .Concat(new [] { comment });

            var sgfValue = new SgfText(string.Join("\n", comments), false);
            node["C"] = new [] { sgfValue };
        }
    }
}
