using Haengma.Core.Logics.Games;
using Haengma.Core.Models;
using Haengma.Core.Persistence;
using Haengma.Core.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Haengma.Core.Logics.Lobby
{
    public record LobbyLogicContext(
        LobbyState Lobby,
        ILobbyNotifier Notifier,
        IIdGenerator<string> IdGenerator,
        GameLogicContext Games,
        Random Randomizer
    )
    {
        public void AddConnectedUser(UserId userId) => Lobby.ConnectedUsers.Add(userId);

        public void RemoveConnectedUserById(UserId userId) => Lobby.ConnectedUsers.Remove(userId);

        public GameId CreateGame(UserId userId, GameSettings gameSettings)
        {
            var gameId = new GameId(IdGenerator.Generate());
            Lobby.OpenGames[userId] = new OpenGameState(gameId, gameSettings);
            return gameId;
        }

        public async Task StartGameAsync(ITransaction transaction, UserId userId, UserId challengerId)
        {
            var game = Lobby.OpenGames[userId] ?? throw new ArgumentException($"There's no open game for the user {userId}.");
            var players = DecidePlayers(game.Settings.ColorDecision, userId, challengerId);
            await Games.CreateGameAsync(transaction, game.GameId, game.Settings, players.Black, players.White);
        }

        public GameSettings? GetGameById(GameId gameId) => Lobby
            .OpenGames
            .Values
            .Where(x => x.GameId == gameId)
            .Select(x => x.Settings)
            .SingleOrDefault();

        public UserId? GetOwnerByGameId(GameId id) => Lobby
            .OpenGames
            .Where(x => x.Value.GameId == id)
            .Select(x => x.Key)
            .SingleOrDefault();

        private DecidedPlayers DecidePlayers(ColorDecision decision, UserId ownerId, UserId challengerId) => decision switch
        {
            ColorDecision.Nigiri => DecideByNigiri(ownerId, challengerId),
            ColorDecision.OwnerTakesBlack => new(ownerId, challengerId),
            ColorDecision.ChallengerTakesBlack => new(challengerId, ownerId),
            _ => throw new ArgumentOutOfRangeException(nameof(decision), decision, "Couldn't recognize the decision.")
        };

        private DecidedPlayers DecideByNigiri(UserId u1, UserId u2)
        {
            var users = new[] { u1, u2 };
            var black = Randomizer.Next(users);
            var white = users.Except(new[] { black }).Single();

            return new(black, white);
        }

        private record DecidedPlayers(UserId Black, UserId White);
    }
}
