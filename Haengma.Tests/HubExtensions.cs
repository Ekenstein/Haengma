using Haengma.GS.Hubs;
using Haengma.GS.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Haengma.Tests
{
    public static class HubExtensions
    {
        public static Mock<IGameClient> CreateGameClient(this HubConnection connection)
        {
            var client = new Mock<IGameClient>();
            connection.On<JsonBoard>(nameof(IGameClient.BoardUpdated), v => client.Object.BoardUpdated(v));
            connection.On<string>(nameof(IGameClient.CommentAdded), v => client.Object.CommentAdded(v));
            connection.On<string>(nameof(IGameClient.GameCreated), v => client.Object.GameCreated(v));
            connection.On<JsonGame>(nameof(IGameClient.GameStarted), v => client.Object.GameStarted(v));
            connection.On<JsonColor>(nameof(IGameClient.PlayerPassed), v => client.Object.PlayerPassed(v));

            return client;
        }

        public static Task CreateGameAsync(this HubConnection hubConnection, JsonGameSettings gameSettings) => hubConnection
            .InvokeAsync(nameof(GameHub.CreateGame), gameSettings);

        public static Task JoinGameAsync(this HubConnection hubConnection, string gameId) => hubConnection
            .InvokeAsync(nameof(GameHub.JoinGame), gameId);

        public static Task AddMoveAsync(this HubConnection hubConnection, string gameId, JsonPoint move) => hubConnection
            .InvokeAsync(nameof(GameHub.AddMove), gameId, move);

        public static Task PassAsync(this HubConnection hubConnection, string gameId) => hubConnection
            .InvokeAsync(nameof(GameHub.Pass), gameId);

        public static async Task<JsonColor> VerifyPlayerPassedAsync(this Mock<IGameClient> gameClient, Times times, string failMessage = "Couldn't verify that a player passed.")
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.PlayerPassed(It.IsAny<JsonColor>()), times, failMessage: failMessage);
            var value = gameClient.GetValueFromInvocations<JsonColor>(nameof(IGameClient.PlayerPassed));
            gameClient.Invocations.Clear();
            return value;
        }

        public static async Task<JsonGame> VerifyGameStarted(this Mock<IGameClient> gameClient, Times times, string failMessage = "Couldn't verify that game started was triggered.")
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.GameStarted(It.IsAny<JsonGame>()), times, failMessage: failMessage);
            var value = gameClient.GetValueFromInvocations<JsonGame>(nameof(IGameClient.GameStarted));
            gameClient.Invocations.Clear();
            return value;
        }

        public static async Task<JsonBoard> VerifyBoardUpdated(this Mock<IGameClient> gameClient, Times times, string failMessage = "Couldn't verify board update.")
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.BoardUpdated(It.IsAny<JsonBoard>()), times, failMessage: failMessage);
            var value = gameClient.GetValueFromInvocations<JsonBoard>(nameof(IGameClient.BoardUpdated));
            gameClient.Invocations.Clear();
            return value;
        }

        public static async Task<string> VerifyGameCreated(this Mock<IGameClient> gameClient, Times times, string failMessage = "Couldn't verify game created.")
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.GameCreated(It.IsAny<string>()), times, failMessage: failMessage);
            var value = gameClient.GetValueFromInvocations<string>(nameof(IGameClient.GameCreated));
            gameClient.Invocations.Clear();
            return value;
        }

        private static T GetValueFromInvocations<T>(this Mock<IGameClient> client, string methodName) => client
            .Invocations
            .Where(x => x.Method == typeof(IGameClient).GetMethod(methodName))
            .SelectMany(x => x.Arguments)
            .OfType<T>()
            .LastOrDefault();
    
        private static async Task VerifyWithTimeoutAsync(this Mock<IGameClient> gameClient,
            Expression<Func<IGameClient, Task>> method,
            Times times,
            int timeOutInMs = 100,
            int delayBetweenIterationInMs = 50,
            string failMessage = "Failed to verify")
        {
            bool hasBeenExecuted = false;
            bool hasTimedOut = false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!hasBeenExecuted && !hasTimedOut)
            {
                if (stopwatch.ElapsedMilliseconds > timeOutInMs)
                {
                    hasTimedOut = true;
                }

                try
                {
                    gameClient.Verify(method, times);
                    hasBeenExecuted = true;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
                await Task.Delay(delayBetweenIterationInMs);
            }

            if (!hasBeenExecuted)
            {
                Assert.False(true, failMessage);
            }
        }
    }
}
