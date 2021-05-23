using Haengma.Core.Models;
using System.Collections.Generic;

namespace Haengma.Core.Logics.Lobby
{
    public record LobbyState(
        ISet<UserId> ConnectedUsers,
        IDictionary<UserId, OpenGameState> OpenGames,
        IDictionary<GameId, Game> ActiveGames
    );
}
