using Haengma.Core.Models;
using Haengma.Core.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Haengma.Core.Models.TimeSettings;

namespace Haengma.Core.Persistence
{
    public static class GameRepository
    {
        private static IQueryable<InternalGame> Games(this IReadOnlyTransaction transaction) => transaction.Query<InternalGame>();

        private static IQueryable<InternalGame> GameById(this IReadOnlyTransaction transaction, GameId id) => transaction.Games().Where(x => x.Id == id);

        private static InternalGame ToDatabaseModel(this Game game) => new(
            game.Id,
            game.BlackPlayer,
            game.WhitePlayer,
            game.GameSettings.BoardSize,
            game.GameSettings.Komi,
            game.GameSettings.Handicap,
            game.GameSettings.TimeSettings switch
            {
                ByoYomi => TimeSettingsType.ByoYomi,
                _ => throw new ArgumentOutOfRangeException(nameof(game.GameSettings.TimeSettings), game.GameSettings.TimeSettings, "Couldn't recognize the given time settings.")
            },
            game.GameSettings.TimeSettings.MainTimeInSeconds,
            game.GameSettings.TimeSettings switch
            {
                ByoYomi byoYomi => byoYomi.ByoYomiPeriods,
                _ => 0
            },
            game.GameSettings.TimeSettings switch
            {
                ByoYomi byoYomi => byoYomi.ByoYomiSeconds,
                _ => 0
            },
            game.Sgf,
            game.GameSettings.ColorDecision
        );

        private static Game ToServiceModel(this InternalGame game) => new(
            Id: game.Id,
            BlackPlayer: game.BlackPlayer,
            WhitePlayer: game.WhitePlayer,
            Sgf: game.Sgf,
            GameSettings: new GameSettings(
                BoardSize: game.BoardSize,
                Komi: game.Komi,
                Handicap: game.Handicap,
                TimeSettings: game.TimeSettingsType switch
                {
                    TimeSettingsType.ByoYomi => new ByoYomi(
                        MainTimeInSeconds: game.MainTimeInSeconds,
                        ByoYomiPeriods: game.ByoYomiPeriods,
                        ByoYomiSeconds: game.ByoYomiSeconds
                    ),
                    _ => throw new InvalidOperationException("The game contains a time settings type that couldn't be recognized.")
                },
                ColorDecision: game.ColorDecision
            )
        );

        public static async Task<Game> GetGameByIdAsync(this IReadOnlyTransaction transaction, GameId id)
        {
            var game = await transaction.GameById(id).SingleOrDefaultAsync();

            return game.ToServiceModel();
        }

        public static void AddGame(this ITransaction transaction, Game game) => transaction.Add(game.ToDatabaseModel());

        public static async Task UpdateSgfAsync(this ITransaction transaction, GameId id, string sgf)
        {
            var game = await transaction.GameById(id).SingleOrDefaultAsync() ?? throw new NoSuchEntityException(id);

            transaction.Update(game with
            {
                Sgf = sgf
            });
        }
    }
}
