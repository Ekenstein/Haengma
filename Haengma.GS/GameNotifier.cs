using Haengma.Core.Logics.Games;
using Haengma.Core.Logics.Lobby;
using Haengma.Core.Models;
using Haengma.GS.Hubs;
using Haengma.GS.Models;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace Haengma.GS
{
    public class GameNotifier : IGameNotifier
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public GameNotifier(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task CommentAddedAsync(GameId gameId, string comment) => _hubContext.Clients.Group(gameId.Value).CommentAdded(comment);

        public Task GameHasEndedAsync(GameId gameId) => _hubContext.Clients.Group(gameId.Value).GameEnded();

        public async Task GameHasStartedAsync(GameId gameId, UserId black, UserId white, Board board)
        {
            await AddPlayersToGame(gameId, black, white);
            await NotifyGameStarted(black, board, Color.Black);
            await NotifyGameStarted(white, board, Color.White);
        }

        private Task NotifyGameStarted(UserId user, Board board, Color color) => _hubContext
            .Clients
            .Client(user.Value)
            .GameStarted(new JsonGame(board.ToJson(), color.ToJson()));

        private Task AddPlayersToGame(GameId gameId, params UserId[] userId)
        {
            var tasks = userId.Select(x => _hubContext.Groups.AddToGroupAsync(x.Value, gameId.Value));
            return Task.WhenAll(tasks);
        }

        public Task PlayerPassedAsync(GameId gameId, Color color) => _hubContext.Clients.Group(gameId.Value).PlayerPassed(color.ToJson());

        public Task PlayerResignedAsync(GameId gameId, Color color) => _hubContext.Clients.Group(gameId.Value).PlayerResigned(color.ToJson());

        public Task MoveAddedAsync(GameId gameId, Board board) => _hubContext.Clients.Group(gameId.Value).BoardUpdated(board.ToJson());
    }

    public class LobbyNotifier : ILobbyNotifier
    {
        private readonly IHubContext<GameHub, IGameClient> _context;

        public LobbyNotifier(IHubContext<GameHub, IGameClient> context)
        {
            _context = context;
        }

        public Task ChallengeCancelled(UserId owner, UserId challenger)
        {
            throw new System.NotImplementedException();
        }

        public Task ChallengeDeclinedAsync(UserId owner, UserId challenger, GameId gameId)
        {
            throw new System.NotImplementedException();
        }

        public Task ChallengeReceivedAsync(GameId gameId, UserId owner, UserId challenger, GameSettings gameSettings)
        {
            throw new System.NotImplementedException();
        }

        public Task GameSettingsUpdated(GameId gameId, GameSettings gameSettings)
        {
            throw new System.NotImplementedException();
        }
    }
}
