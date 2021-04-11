using Haengma.Backend.Imperative.Models;
using Haengma.Backend.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Backend.Imperative.Persistance.Repositories
{
    public static class GameRepository
    {
        public static IReadOnlyList<Game> GetGamesByUserId(this IReadOnlyTransaction transaction, UserId userId) => transaction
            .Query<Game>()
            .Where(x => x.Black == userId || x.White == userId)
            .ToArray();

        public static Game GetGameById(this IReadOnlyTransaction transaction, GameId id) => transaction
            .Query<Game>()
            .GetOrThrowResourceNotFound(id);

        public static void InsertGame(this IWritableTransaction transaction, Game game)
        {
            transaction.Add(game);
            transaction.Commit();
        }

        public static void UpdateSgfForGame(this IWritableTransaction transaction, GameId id, string sgf)
        {
            var game = transaction.GetGameById(id);
            transaction.Update(game with
            {
                Sgf = sgf
            });
            transaction.Commit();
        }
    }
}
