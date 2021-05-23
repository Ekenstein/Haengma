using Haengma.Core.Models;
using Haengma.GS.Models;
using System;
using static Haengma.Core.Models.Rank;
using static Haengma.Core.Models.TimeSettings;

namespace Haengma.GS
{
    public static class JsonToModelConverter
    {
        public static TimeSettings ToModel(this JsonTimeSettings timeSettings) => timeSettings.Type switch
        {
            JsonTimeSettingType.ByoYomi => new ByoYomi(
                timeSettings.MainTimeInSeconds,
                timeSettings.ByoYomiPeriods,
                timeSettings.ByoYomiSeconds
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(timeSettings.Type), timeSettings.Type, "Couldn't recognize the time setting type.")
        };

        public static ColorDecision ToModel(this JsonPlayerDecision decision) => decision switch
        {
            JsonPlayerDecision.Nigiri => ColorDecision.Nigiri,
            JsonPlayerDecision.OwnerTakesBlack => ColorDecision.OwnerTakesBlack,
            JsonPlayerDecision.ChallengerTakesBlack => ColorDecision.ChallengerTakesBlack,
            _ => throw new ArgumentOutOfRangeException(nameof(decision), decision, "Couldn't recognize the player decision.")
        };

        public static Color ToModel(this JsonColor color) => color switch
        {
            JsonColor.Black => Color.Black,
            JsonColor.White => Color.White,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, $"Couldn't recognize the given color {color}")
        };

        public static Point ToModel(this JsonPoint point) => new(point.X, point.Y);

        public static GameSettings ToModel(this JsonGameSettings gameSettings) => new(
            gameSettings.BoardSize,
            gameSettings.Komi,
            gameSettings.Handicap,
            gameSettings.TimeSettings.ToModel(),
            gameSettings.ColorDecision.ToModel()
        );

        public static Rank ToModel(this JsonRank rank) => rank.RankType switch
        {
            JsonRankType.Kyu => new Kyu
            {
                Value = rank.Rank
            },
            JsonRankType.Dan => new Dan
            {
                Value = rank.Rank
            },
            _ => throw new ArgumentOutOfRangeException(nameof(rank.RankType), rank.RankType, "Couldn't recognize the given rank type.")
        };
    }
}
