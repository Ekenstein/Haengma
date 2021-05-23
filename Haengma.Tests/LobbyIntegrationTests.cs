using Haengma.GS.Hubs;
using Haengma.GS.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Haengma.Tests
{
    public class LobbyIntegrationTests : IntegrationTest
    {
        [Fact]
        public async Task CreatingGameReturnsGameId()
        {
            var hubConnection = NewHubConnection("game");

            var gameSettings = new JsonGameSettings(
                19,
                6.5,
                4,
                new JsonTimeSettings(JsonTimeSettingType.ByoYomi, 60, 5, 30),
                JsonPlayerDecision.OwnerTakesBlack
            );

            var handler = new Mock<Action<string>>();

            hubConnection.On(nameof(IGameClient.GameCreated), handler.Object);

            await hubConnection.InvokeAsync(nameof(GameHub.CreateGame), gameSettings);

            await handler.VerifyWithTimeoutAsync(a => a(It.IsAny<string>()), Times.Once());
        }
    }
}
