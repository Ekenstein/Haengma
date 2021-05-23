using Haengma.Core.Models;

namespace Haengma.Core.Logics.Lobby
{
    public record OpenGameState(
        GameId GameId, 
        GameSettings Settings, 
        ChallengeQueue Challenges
    );
}
