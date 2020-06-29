using Haengma.GIB;
using Haengma.SGF;
using Haengma.SGF.SgfProperties;
using Monadicsh;
using Monadicsh.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.GibToSgfConverter
{
    public static class GibFileExtensions
    {
        public static IEnumerable<SgfProperty> GetBlackPlayerName(this GibFile file) => Maybe
            .CreateNonEmpty(file.BlackName)
            .Map(new SgfProperty[0], x => new [] { new BlackPlayerName(x) });

        public static IEnumerable<SgfProperty> GetWhitePlayerName(this GibFile file) => Maybe
            .CreateNonEmpty(file.WhiteName)
            .Map(new SgfProperty[0], x => new [] { new WhitePlayerName(x) });

        public static IEnumerable<SgfProperty> GetResult(this GibFile file)
        {
            SgfProperty ToSgf(GibResult result) => result switch
            {
                GibResult.BlackWinsByCounting => GameResult.BlackWins(file.Score),
                GibResult.WhiteWinsByCounting => GameResult.WhiteWins(file.Score),
                GibResult.BlackWinsByResignation => GameResult.BlackWinsByResignation,
                GibResult.WhiteWinsByResignation => GameResult.WhiteWinsByResignation,
                GibResult.BlackWinsByTime => GameResult.BlackWinsByResignation,
                GibResult.WhiteWinsByTime => GameResult.WhiteWinsByTime,
                _ => GameResult.Unknown
            };

            return file.Result.Select(r => new [] { ToSgf(r) }, new SgfProperty[0]);
        }

        public static IEnumerable<SgfProperty> GetHandicap(this GibFile file) => new [] { file.Handicap }
            .Where(x => x >= 2)
            .SelectMany(x => new SgfProperty[] 
            {
                new Handicap(x), 
                new AddBlackStones(AddBlackStones.Handicap(x))
            });

        public static IEnumerable<SgfProperty> GetKomi(this GibFile file) => new [] { file.Komi }
            .Select(x => new Komi(x));

        public static IEnumerable<SgfProperty> GetGamePlace(this GibFile file) => new [] { file.GamePlace }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => new GamePlace(x));

        public static IEnumerable<SgfProperty> GetGameDate(this GibFile file)
        {
            if (file.Date.HasValue)
            {
                yield return new GameDate(file.Date.Value);
            }
        }
    }
}
