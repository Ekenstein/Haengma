using Haengma.Core.Models;
using System.Threading.Tasks;

namespace Haengma.Core.Logics.Games
{
    public interface IGameNotifier
    {
        Task MoveAddedAsync(GameId gameId, Board board);
        Task PlayerPassedAsync(GameId gameId, Color color);
        Task PlayerResignedAsync(GameId gameId, Color color);
        Task GameHasEndedAsync(GameId gameId);
        Task GameHasStartedAsync(GameId gameId, UserId black, UserId white, Board board);
        Task CommentAddedAsync(GameId gameId, string comment);
        Task EmoteSentAsync(GameId gameId, Emote emote, Color sender);
    }
}
