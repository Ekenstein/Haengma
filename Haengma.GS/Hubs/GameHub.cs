using Haengma.GS.Actions;
using Haengma.GS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Haengma.GS.Hubs
{
    [AllowAnonymous]
    [DisableCors]
    public class GameHub : Hub<IGameClient>
    {
        private readonly ActionContext _actionContext;

        public GameHub(ActionContext actionContext)
        {
            _actionContext = actionContext;
        }

        public Task AddMove(string gameId, JsonPoint point) => _actionContext.AddMoveAsync(gameId, Context.ConnectionId, point);
        public Task Pass(string gameId) => _actionContext.PassAsync(gameId, Context.ConnectionId);
        public Task Resign(string gameId) => _actionContext.ResignAsync(gameId, Context.ConnectionId);
        public Task Comment(string gameId, string message) => _actionContext.CommentAsync(gameId, Context.ConnectionId, message);

        public async Task CreateGame(JsonGameSettings gameSettings)
        {
            var gameId = _actionContext.CreateGame(Context.ConnectionId, gameSettings);
            await Clients.Caller.GameCreated(gameId);
        }

        public Task JoinGame(string gameId) => _actionContext.JoinGameAsync(gameId, Context.ConnectionId);
    }

    public interface IGameClient
    {
        Task GameCreated(string gameId);
        Task BoardUpdated(JsonBoard board);
        Task GameStarted(JsonGame game);
        Task GameEnded();
        Task PlayerResigned(JsonColor color);
        Task PlayerPassed(JsonColor color);
        Task CommentAdded(string comment);
    }
}
