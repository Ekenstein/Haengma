﻿using Haengma.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Haengma.Core.Sgf
{
    public static class SgfWriter
    {
        public static string ToSgf(this IEnumerable<SgfGameTree> collection)
        {
            var sb = new StringBuilder();
            collection.ForEach(x => sb.Append(x.ToSgf()));
            return sb.ToString();
        }

        public static string ToSgf(this SgfGameTree tree)
        {
            var sb = new StringBuilder();
            sb.Append('(');
            tree.Sequence.ForEach(x => sb.Append(x.ToSgf()));
            tree.Trees.ForEach(x => sb.Append(x.ToSgf()));
            sb.Append(')');
            return sb.ToString();
        }

        public static string ToSgf(this SgfNode node)
        {
            var sb = new StringBuilder();
            sb.Append(';');
            foreach (var property in node.Properties)
            {
                sb.Append(property.ToSgf());
            }
            return sb.ToString();
        }

        public static string ToSgf(this SgfProperty property) => property.Accept(new SgfPropertyToSgf());

        private static string ToSgf(this Move move) => move
            .ToPoint()
            ?.Let(p => p.ToSgf()) ?? "";

        private static string EscapeChar(char c, bool isComposed)
        {
            var needsEscape = new[] { ']', '\\' }.Union(new[] { ':' }.Where(_ => isComposed));
            return needsEscape.Contains(c)
                ? $"\\{c}"
                : c.ToString();
        }

        private static string SimpleTextToSgf(this SgfSimpleText text, bool isComposed) => text
            .Text
            .SelectMany(x => EscapeChar(x, isComposed))
            .JoinToString("");

        private static string TextToSgf(this SgfText text, bool isComposed) => text
            .Text
            .Select(x => EscapeChar(x, isComposed))
            .JoinToString("");

        private static string ToSgf(this SgfPoint point)
        {
            static char IntToChar(int n)
            {
                if (n > 26)
                {
                    return (char)((n % 27) + 'A');
                }
                else
                {
                    return (char)((n - 1) + 'a');
                }
            }

            return $"{IntToChar(point.X)}{IntToChar(point.Y)}";
        }

        private static string ToSgf(this SgfColor color) => color switch
        {
            SgfColor.Black => "B",
            SgfColor.White => "W",
            _ => throw new InvalidOperationException("")
        };

        private static string ToSgf<L, R>(this (L, R) value, Func<L, string> left, Func<R, string> right)
        {
            var sb = new StringBuilder();
            sb.Append(left(value.Item1));
            sb.Append(':');
            sb.Append(right(value.Item2));
            return sb.ToString();
        }

        private class SgfPropertyToSgf : ISgfPropertyVisitor<string>
        {
            private static string ToSgf(string identifier, params string[] values)
            {
                var sb = new StringBuilder();
                sb.Append(identifier);
                if (values == null || values.Length <= 0)
                {
                    sb.Append("[]");
                }
                else
                {
                    foreach (var value in values)
                    {
                        sb.Append($"[{value}]");
                    }
                }

                return sb.ToString();
            }

            public string Accept(SgfProperty.B b) => ToSgf("B", b.Move.ToSgf());

            public string Accept(SgfProperty.W w) => ToSgf("W", w.Move.ToSgf());

            public string Accept(SgfProperty.C c) => ToSgf("C", c.Comment.TextToSgf(false));

            public string Accept(SgfProperty.PB pB) => ToSgf("PB", pB.Name.SimpleTextToSgf(false));

            public string Accept(SgfProperty.PW pW) => ToSgf("PW", pW.Name.SimpleTextToSgf(false));

            public string Accept(SgfProperty.AB aB) => ToSgf("AB", aB.Stones.Select(x => x.ToSgf()).ToArray());

            public string Accept(SgfProperty.AW aW) => ToSgf("AW", aW.Stones.Select(x => x.ToSgf()).ToArray());

            public string Accept(SgfProperty.SZ sZ) => ToSgf("SZ", sZ.Size.ToString());

            public string Accept(SgfProperty.HA hA) => ToSgf("HA", hA.Handicap.ToString());

            public string Accept(SgfProperty.MN mN) => ToSgf("MN", mN.MoveNumber.ToString());

            public string Accept(SgfProperty.KM kM) => ToSgf("KM", kM.Komi.ToString(CultureInfo.InvariantCulture));

            public string Accept(SgfProperty.PL pL) => ToSgf("PL", pL.Color.ToSgf());

            public string Accept(SgfProperty.AP aP) => ToSgf(
                "AP",
                aP.Name.To(aP.Version).ToSgf(x => x.SimpleTextToSgf(true), x => x.SimpleTextToSgf(true))
            );

            public string Accept(SgfProperty.Unknown unknown) => ToSgf(
                unknown.Identifier, 
                unknown.Values.Select(x => TextToSgf(x, false)).ToArray()
            );

            public string Accept(SgfProperty.BR bR) => ToSgf("BR", bR.Rank.SimpleTextToSgf(false));

            public string Accept(SgfProperty.WR wR) => ToSgf("WR", wR.Rank.SimpleTextToSgf(false));

            public string Accept(SgfProperty.OT oT) => ToSgf("OT", oT.Overtime.SimpleTextToSgf(false));

            public string Accept(SgfProperty.RE rE) => ToSgf("RE", rE.Result.SimpleTextToSgf(false));

            public string Accept(SgfProperty.EM emote) => ToSgf("EM", (emote.Color, emote.Message).ToSgf(x => x.ToSgf(), x => ((int)x).ToString()).ToString());
        }
    }
}
