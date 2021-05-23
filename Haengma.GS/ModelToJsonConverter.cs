using Haengma.Core.Models;
using Haengma.GS.Models;
using System;
using System.Linq;
using static Haengma.Core.Models.Rank;
using static Haengma.Core.Models.TimeSettings;

namespace Haengma.GS
{
    public static class ModelToJsonConverter
    {
        public static JsonGameId ToJson(this GameId gameId) => new(gameId.Value);

        public static JsonRank ToJson(this Rank rank) => rank switch
        {
            Dan => new JsonRank(rank.Value, JsonRankType.Dan),
            Kyu => new JsonRank(rank.Value, JsonRankType.Kyu),
            _ => throw new InvalidOperationException($"Couldn't recognize the given rank {rank}.")
        };

        public static JsonTimeSettings ToJson(this TimeSettings timeSettings) => timeSettings switch
        {
            ByoYomi byoYomi => new JsonTimeSettings(JsonTimeSettingType.ByoYomi, byoYomi.MainTimeInSeconds, byoYomi.ByoYomiPeriods, byoYomi.ByoYomiSeconds),
            _ => throw new InvalidOperationException($"Couldn't recognize the given time setting {timeSettings}.")
        };

        public static JsonGameSettings ToJson(this GameSettings gameSettings) => new(
            gameSettings.BoardSize, 
            gameSettings.Komi, 
            gameSettings.Handicap, 
            gameSettings.TimeSettings.ToJson(),
            gameSettings.ColorDecision.ToJson()
        );

        public static JsonColor ToJson(this Color color) => color switch
        {
            Color.Black => JsonColor.Black,
            Color.White => JsonColor.White,
            _ => throw new InvalidOperationException($"Couldn't recognize the given color {color}.")
        };

        public static JsonStone ToJson(this Stone stone) => new(stone.Color.ToJson(), stone.Point.ToJson());

        public static JsonBoard ToJson(this Board board) => new(
            board.WhiteCaptures,
            board.BlackCaptures,
            board.GameSettings.ToJson(),
            board.Stones.Select(x => x.ToJson()),
            board.Next.ToJson()
        );

        public static JsonPoint ToJson(this Point point) => new(point.X, point.Y);

        public static JsonPlayerDecision ToJson(this ColorDecision decision) => decision switch
        {
            ColorDecision.Nigiri => JsonPlayerDecision.Nigiri,
            ColorDecision.OwnerTakesBlack => JsonPlayerDecision.OwnerTakesBlack,
            ColorDecision.ChallengerTakesBlack => JsonPlayerDecision.ChallengerTakesBlack,
            _ => throw new InvalidOperationException($"Couldn't recognize the given decision {decision}.")
        };
    }
}
