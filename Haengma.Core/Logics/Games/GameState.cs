using Haengma.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Haengma.Core.Logics.Games
{
    public record GameState(IReadOnlyDictionary<UserId, Color> Players, SemaphoreSlim Lock);
    public static class GameStateExtensions
    {
        public static Color GetPlayerColor(this GameState gameState, UserId userId) => gameState.Players.TryGetValue(userId, out var color)
            ? color
            : throw new ArgumentException($"There's no player in the game corresponding to the user id {userId.Value}.");
    }
}
