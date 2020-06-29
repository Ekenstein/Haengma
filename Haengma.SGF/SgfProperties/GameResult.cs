using Haengma.SGF.ValueTypes;
using System.Globalization;

namespace Haengma.SGF.SgfProperties
{
    public class GameResult : SgfProperty
    {
        public static GameResult Draw => new GameResult("0");
        public static GameResult BlackWins(double? score) => new GameResult("B+" + score?.ToString("N2", CultureInfo.InvariantCulture) ?? "");
        public static GameResult WhiteWins(double? score) => new GameResult("W+" + score?.ToString("N2", CultureInfo.InvariantCulture) ?? "");
        public static GameResult BlackWinsByTime => new GameResult("B+T");
        public static GameResult WhiteWinsByTime => new GameResult("W+T");
        public static GameResult BlackWinsByResignation => new GameResult("B+R");
        public static GameResult WhiteWinsByResignation => new GameResult("W+R");
        public static GameResult BlackWinsByForfeit => new GameResult("B+F");
        public static GameResult WhiteWinsByForfeit => new GameResult("W+F");
        public static GameResult Unknown => new GameResult("?");
        public static GameResult NoResult => new GameResult("Void");

        private GameResult(string result) : base("RE", new SgfSimpleText(result, false))
        {
        }
    }
}
