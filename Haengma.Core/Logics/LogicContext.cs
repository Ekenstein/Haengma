using Haengma.Core.Logics.Games;
using Haengma.Core.Logics.Lobby;
using Haengma.Core.Models;
using System;
using System.Collections.Generic;

namespace Haengma.Core.Logics
{
    public class LogicContext
    {
        public LogicContext(LobbyState lobbyState,
            IDictionary<GameId, GameState> games,
            ILobbyNotifier lobbyNotifier,
            IGameNotifier gameNotifier,
            IIdGenerator<string> idGenerator,
            Random randomizer)
        {
            Game = new GameLogicContext(gameNotifier, games);
            Lobby = new LobbyLogicContext(lobbyState, lobbyNotifier, idGenerator, Game, randomizer);
        }

        public GameLogicContext Game { get; }
        public LobbyLogicContext Lobby { get; }
    }
}
