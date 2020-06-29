using System;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.GIB
{
    public class GibFile
    {
        public GibFile(IDictionary<string, IEnumerable<GibPropertyValue>> header, GibNode[] gameTree)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            GameTree = gameTree ?? throw new ArgumentNullException(nameof(gameTree));
        }

        private IDictionary<string, IEnumerable<GibPropertyValue>> Header { get; }
        private GibNode[] GameTree { get; }

        public IEnumerable<GibPropertyValue> this[string property] => Header.TryGetValue(property, out var values)
            ? values
            : new GibPropertyValue[0];

        public string? GamePlace => this["GAMEPLACE"]
            .Select(x => x.Value)
            .SingleOrDefault();

        private int[] GetTimeParts() => this["GAMEINFOMAIN"]
            .Where(x => x.Name == "GTIME")
            .Select(x => x.Value)
            .SelectMany(x => x.Split("-"))
            .Select(int.Parse)
            .ToArray();

        public int MainTime => GetTimeParts()[0];

        public int ByoYomiPeriods => GetTimeParts()[2];

        public int ByoYomiTime => GetTimeParts()[1];

        public string? BlackName => this["GAMEBLACKNICK"]
            .Select(x => x.Value)
            .SingleOrDefault();

        public string? WhiteName => this["GAMEWHITENICK"]
            .Select(x => x.Value)
            .SingleOrDefault();

        public DateTime? Date => this["GAMETAG"]
            .Select(x => x.ValueAsDate("Cyyyy:MM:dd:HH:mm"))
            .Where(x => x.HasValue)
            .SingleOrDefault();

        public double Komi => this["GAMEINFOMAIN"]
            .Where(x => x.Name == "GONGJE")
            .Select(x => x.ValueAsNumber())
            .SingleOrDefault()
            .Select(n => (double) n / 10, 0);

        public double Score => this["GAMEINFOMAIN"]
            .Where(x => x.Name == "ZIPSU")
            .Select(x => x.ValueAsNumber())
            .SingleOrDefault()
            .Select(n => (double) n / 10, 0);

        private GibResult? ToResult(int value) => value switch
        {
            0 => GibResult.BlackWinsByCounting,
            1 => GibResult.WhiteWinsByCounting,
            3 => GibResult.BlackWinsByResignation,
            4 => GibResult.WhiteWinsByResignation,
            7 => GibResult.BlackWinsByTime,
            8 => GibResult.WhiteWinsByTime,
            _ => null
        };

        public int Handicap => GameTree
            .Where(x => x.Values[0] == "INI")
            .Select(x => x.Values[3])
            .Select(int.Parse)
            .SingleOrDefault();

        public GibResult? Result => this["GAMEINFOMAIN"]
            .Where(x => x.Name == "GRLT")
            .Select(x => x.ValueAsNumber())
            .SingleOrDefault()
            .Select(ToResult, default);

        public IOrderedEnumerable<GibStone> Stones => GameTree
            .Where(x => x.Values[0] == "STO")
            .Select(x => new GibStone(x.Values))
            .OrderBy(x => x.MoveNumber);
    }
}
