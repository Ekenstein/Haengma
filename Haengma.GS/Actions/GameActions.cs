using Haengma.Core.Services;
using Haengma.GS.Models;
using System.Threading.Tasks;

namespace Haengma.GS.Actions
{
    public static class GameActions
    {
        public static Task AddMoveAsync(this ActionContext context, string gameId, string connectionId, JsonPoint point) => context.Services.AddMoveAsync(
            gameId: new(gameId),
            userId: new(connectionId),
            point: point.ToModel()
        );

        public static Task PassAsync(this ActionContext context, string gameId, string connectionId) => context.Services.PassAsync(
            gameId: new(gameId),
            userId: new(connectionId)
        );

        public static Task ResignAsync(this ActionContext context, string gameId, string connectionId) => context.Services.ResignAsync(
            gameId: new(gameId),
            userId: new(connectionId)
        );

        public static Task CommentAsync(this ActionContext context, string gameId, string connectionId, string comment) => context.Services.AddCommentAsync(
            gameId: new(gameId),
            userId: new(connectionId),
            comment: comment
        );
    }
}
