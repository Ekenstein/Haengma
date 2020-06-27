using Haengma.SGF.Commons;
using Haengma.SGF.ValueTypes;
using Pidgin;
using System.Collections.Generic;
using static Haengma.SGF.Parser.SgfParser;

namespace Haengma.SGF.Parser
{
    public static class SgfPropertyParsers
    {
        public static readonly IDictionary<UpperCaseLetterString, Parser<char, SgfValue>> FF4 = new Dictionary<UpperCaseLetterString, Parser<char, SgfValue>>
        {
            { "HA", Number },
            { "KM", Real },
            { "TB", ListOfPoints },
            { "TW", ListOfPoints },
            { "B", Point },
            { "W", Point },
            { "KO", None },
            { "MN", Number },
            { "AB", ListOfPoints },
            { "AE", ListOfPoints },
            { "AW", ListOfPoints },
            { "PL", Color },
            { "C", Text(false) },
            { "DM", Double },
            { "GB", Double },
            { "GW", Double },
            { "HO", Double },
            { "N", SimpleText(false) },
            { "UC", Double },
            { "V", Real },
            { "BM", Double },
            { "DO", None },
            { "IT", None },
            { "TE", Double },
            { "AR", Composed(Point, Point) },
            { "CR", ListOfPoints },
            { "DD", ListOfPoints },
            { "LB", Composed(Point, SimpleText(true)) },
            { "LN", Composed(Point, Point) },
            { "MA", ListOfPoints },
            { "SL", ListOfPoints },
            { "SQ", ListOfPoints },
            { "TR", ListOfPoints },
            { "AP", Composed(SimpleText(true), SimpleText(true)).Or(SimpleText(false)) },
            { "CA", SimpleText(false) },
            { "FF", NumberWithBoundaries(1, 4) },
            { "GM", NumberWithBoundaries(1, 16) },
            { "ST", NumberWithBoundaries(0, 3) },
            { "SZ", Number.Or(Composed(Number, Number)) },
            { "AN", SimpleText(false) },
            { "BR", SimpleText(false) },
            { "BT", SimpleText(false) },
            { "CP", SimpleText(false) },
            { "DT", SimpleText(false) },
            { "EV", SimpleText(false) },
            { "GN", SimpleText(false) },
            { "GC", Text(false) },
            { "ON", SimpleText(false) },
            { "OT", SimpleText(false) },
            { "PB", SimpleText(false) },
            { "PC", SimpleText(false) },
            { "PW", SimpleText(false) },
            { "RE", SimpleText(false) },
            { "RO", SimpleText(false) },
            { "RU", SimpleText(false) },
            { "SO", SimpleText(false) },
            { "TM", Real },
            { "US", SimpleText(false) },
            { "WR", SimpleText(false) },
            { "WT", SimpleText(false) },
            { "BL", Real },
            { "OB", Number },
            { "OW", Number },
            { "WL", Real },
            { "FG", None.Or(Composed(Number, SimpleText(true))) },
            { "PM", Number },
            { "VW", ListOfPoints }
        };
    }
}
