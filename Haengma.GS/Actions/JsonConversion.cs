using Haengma.Backend.Functional.Sgf;
using Haengma.Backend.Imperative.Models;
using Haengma.Backend.Imperative.Services.Models;
using Haengma.Backend.Utils;
using Haengma.GS.Models;
using System;

namespace Haengma.GS.Actions
{
    public static class JsonConversion
    {
        public static GameOptions ToModel(this JsonPostGameOptions options) => new(
            options.BoardSize,
            options.Handicap,
            options.Komi,
            new UserId(options.BlackUserId),
            new UserId(options.WhiteUserId)
        );

        public static Color ToModel(this JsonColor color) => color switch
        {
            JsonColor.White => Color.White,
            JsonColor.Black => Color.Black,
            _ => throw new ArgumentException("wat")
        };

        public static Point ToModel(this JsonPoint point) => new(point.X, point.Y);

        public static Move ToMove(this JsonPostMove move)
        {
            if (move.Pass)
            {
                return new Move.Pass();
            }

            return move.Point?.Map(x => new Move.Point(x.X, x.Y)) ?? throw new ArgumentException("wat");
        }

        public static JsonUser ToJson(this User user) => new(user.Id.Value, user.Name);

        public static JsonGame ToJson(this GameInfo game) => new(
            game.BoardSize, 
            game.Komi, 
            game.Handicap, 
            game.BlackUser.Id, 
            game.WhiteUser.Id,
            game.Sgf);
    }
}
