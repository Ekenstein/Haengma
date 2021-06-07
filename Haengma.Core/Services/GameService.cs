using Haengma.Core.Models;
using Pidgin;
using System.Threading.Tasks;

namespace Haengma.Core.Services
{
    public static class GameService
    {
        public static Task CreateGameAsync(this ServiceContext serviceContext, GameSettings gameSettings, UserId black, UserId white) => serviceContext.Transactions.WriteAndReturnAsync(async tx =>
        {
            var gameId = new GameId(serviceContext.IdGenerator.Generate());
            await serviceContext.Logics.Game.CreateGameAsync(tx, gameId, gameSettings, black, white);
            return Unit.Value;
        });

        public static Task AddMoveAsync(this ServiceContext serviceContext, GameId gameId, UserId userId, Point point) => serviceContext.Transactions.WriteAndReturnAsync(async tx =>
        {
            await serviceContext.Logics.Game.AddMoveAsync(tx, gameId, userId, point);
            return Unit.Value;
        });

        public static Task PassAsync(this ServiceContext serviceContext, GameId gameId, UserId userId) => serviceContext.Transactions.WriteAndReturnAsync(async tx =>
        {
            await serviceContext.Logics.Game.PassAsync(tx, gameId, userId);
            return Unit.Value;
        });

        public static Task AddCommentAsync(this ServiceContext serviceContext, GameId gameId, string comment) => serviceContext.Transactions.WriteAndReturnAsync(async tx =>
        {
            await serviceContext.Logics.Game.AddCommentAsync(tx, gameId, comment);
            return Unit.Value;
        });

        public static Task ResignAsync(this ServiceContext serviceContext, GameId gameId, UserId userId) => serviceContext.Transactions.WriteAndReturnAsync(async tx =>
        {
            await serviceContext.Logics.Game.ResignAsync(tx, gameId, userId);
            return Unit.Value;
        });
    }
}
