using Haengma.Core.Models;
using Haengma.GS.Models;
using System;
using System.Linq;
using static Haengma.Core.Models.TimeSettings;

namespace Haengma.GS
{
    public static class ModelToJsonConverter
    {
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

        public static JsonEmote ToJson(this Emote emote) => emote switch
        {
            Emote.Greetings => JsonEmote.Greetings,
            Emote.Bye => JsonEmote.Bye,
            Emote.Mistake => JsonEmote.Mistake,
            Emote.Impressed => JsonEmote.Impressed,
            Emote.Thanks => JsonEmote.Thanks,
            _ => throw new ArgumentOutOfRangeException(nameof(emote), emote, $"Couldn't recognize the given emote {emote}.")
        };
    }
}
