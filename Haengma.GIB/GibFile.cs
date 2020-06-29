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
            .Where(x => int.TryParse(x, out var _))
            .Select(int.Parse)
            .ToArray();

        private int? GetTimePart(int part)
        {
            var parts = GetTimeParts();
            if (parts.Length != 3)
            {
                return null;
            }

            return parts[part];
        }

        public int? TimeLimit => GetTimePart(0);

        public (int periods, int timePerPeriod)? ByoYomi
        {
            get
            {
                var periods = GetTimePart(2);
                if (periods == null)
                {
                    return default;
                }

                var timePerPeriod = GetTimePart(1);
                if (timePerPeriod == null)
                {
                    return default;
                }

                return (periods.Value, timePerPeriod.Value);
            }
        }

        public string? BlackName => this["GAMEBLACKNICK"]
            .Select(x => x.Value)
            .SingleOrDefault();

        private string? GetRank(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var parts = value.Split(" ");
            if (parts.Length != 2)
            {
                return null;
            }

            var rank = parts[1];
            return rank[1..^1];
        }

        public string? BlackRank => this["GAMEBLACKNAME"]
            .Select(x => x.Value)
            .Select(GetRank)
            .SingleOrDefault();

        public string? WhiteRank => this["GAMEWHITENAME"]
            .Select(x => x.Value)
            .Select(GetRank)
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
