using Haengma.Core.Models;
using System.Threading.Tasks;

namespace Haengma.Core.Services
{
    public static class GameService
    {
        public static Task CreateGameAsync(this ServiceContext serviceContext, GameSettings gameSettings, UserId black, UserId white) => serviceContext
            .Transactions
            .WriteAsync(tx => serviceContext.Logics.Game.CreateGameAsync(tx, new GameId(serviceContext.IdGenerator.Generate()), gameSettings, black, white));

        public static Task AddMoveAsync(this ServiceContext serviceContext, GameId gameId, UserId userId, Point point) => serviceContext
            .Transactions
            .WriteAsync(tx => serviceContext.Logics.Game.AddMoveAsync(tx, gameId, userId, point));

        public static Task PassAsync(this ServiceContext serviceContext, GameId gameId, UserId userId) => serviceContext
            .Transactions
            .WriteAsync(tx => serviceContext.Logics.Game.PassAsync(tx, gameId, userId));

        public static Task AddCommentAsync(this ServiceContext serviceContext, GameId gameId, string comment) => serviceContext
            .Transactions
            .WriteAsync(tx => serviceContext.Logics.Game.AddCommentAsync(tx, gameId, comment));

        public static Task ResignAsync(this ServiceContext serviceContext, GameId gameId, UserId userId) => serviceContext
            .Transactions
            .WriteAsync(tx => serviceContext.Logics.Game.ResignAsync(tx, gameId, userId));
    }
}
