using Haengma.Core.Services;
using Haengma.GS.Models;
using System.Threading.Tasks;

namespace Haengma.GS.Actions
{
    public static class LobbyActions
    {
        public static string CreateGame(this ActionContext context, string connectionId, JsonGameSettings gameSettings)
        {
            var id = context.Services.CreateGame(new(connectionId), gameSettings.ToModel());
            return id.Value;
        }

        public static Task JoinGameAsync(this ActionContext context, string gameId, string connectionId) => context.Services.JoinGameAsync(new(gameId), new(connectionId));
    }
}
