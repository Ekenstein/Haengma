using Haengma.Core.Models;
using System;
using System.Threading.Tasks;

namespace Haengma.Core.Services
{
    public static class LobbyService
    {
        public static void RemoveConnectedUser(this ServiceContext context, UserId userId) => context
            .Logics
            .Lobby
            .RemoveConnectedUserById(userId);

        public static void AddConnectedUser(this ServiceContext context, UserId userId) => context
            .Logics
            .Lobby
            .AddConnectedUser(userId);

        public static GameId CreateGame(this ServiceContext context, UserId userId, GameSettings gameSettings) => context
            .Logics
            .Lobby
            .CreateGame(userId, gameSettings);

        public static async Task JoinGameAsync(this ServiceContext context, GameId gameId, UserId challengerId)
        {
            var ownerId = context.Logics.Lobby.GetOwnerByGameId(gameId)
                ?? throw new ArgumentException($"A game with the id {gameId} doesn't exist.");

            await context.Transactions.WriteAsync(tx => context.Logics.Lobby.StartGameAsync(tx, ownerId, challengerId));
        }
    }
}
