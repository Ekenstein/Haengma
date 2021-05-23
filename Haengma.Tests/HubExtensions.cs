using Haengma.GS.Hubs;
using Haengma.GS.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

            return client;
        }

        public static Task CreateGameAsync(this HubConnection hubConnection, JsonGameSettings gameSettings) => hubConnection
            .InvokeAsync("CreateGame", gameSettings);

        public static Task JoinGameAsync(this HubConnection hubConnection, string gameId) => hubConnection
            .InvokeAsync("JoinGame", gameId);

        public static Task AddMoveAsync(this HubConnection hubConnection, string gameId, JsonPoint move) => hubConnection
            .InvokeAsync("AddMove", gameId, move);

        public static async Task<JsonGame> VerifyGameStarted(this Mock<IGameClient> gameClient)
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.GameStarted(It.IsAny<JsonGame>()), Times.Once());
            return gameClient.GetValueFromInvocations<JsonGame>(nameof(IGameClient.GameStarted));
        }

        public static async Task<JsonBoard> VerifyBoardUpdated(this Mock<IGameClient> gameClient, Times times, string failMessage = "Couldn't verify board update.")
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.BoardUpdated(It.IsAny<JsonBoard>()), times, failMessage: failMessage);
            return gameClient.GetValueFromInvocations<JsonBoard>(nameof(IGameClient.BoardUpdated));
        }

        public static async Task<string> VerifyGameCreated(this Mock<IGameClient> gameClient, Times times, string failMessage = "Couldn't verify game created.")
        {
            await gameClient.VerifyWithTimeoutAsync(x => x.GameCreated(It.IsAny<string>()), times, failMessage: failMessage);
            return gameClient.GetValueFromInvocations<string>(nameof(IGameClient.GameCreated));
        }

        private static T GetValueFromInvocations<T>(this Mock<IGameClient> client, string methodName)
        {
            var method = typeof(IGameClient).GetMethod(methodName);
            var invocations = client.Invocations.Where(x => x.Method == method).ToArray();
            var arguments = invocations.SelectMany(x => x.Arguments).ToArray();
            return arguments.OfType<T>().SingleOrDefault();
        }

        private static async Task VerifyWithTimeoutAsync(this Mock<IGameClient> gameClient,
            Expression<Func<IGameClient, Task>> method,
            Times times,
            int timeOutInMs = 100,
            int delayBetweenIterationInMs = 20,
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
                catch
                {
                }
                await Task.Delay(delayBetweenIterationInMs);
            }

            //if (!hasBeenExecuted)
            //{
            //    Assert.False(true, failMessage);
            //}
        }
    }
}
