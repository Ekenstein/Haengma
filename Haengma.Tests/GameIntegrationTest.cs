using Haengma.GS.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Haengma.Tests
{
    public class GameIntegrationTest : IntegrationTest
    {
        private static readonly JsonTimeSettings TimeSettings = new(JsonTimeSettingType.ByoYomi, 600, 5, 30);
        private static readonly JsonGameSettings GameSettings = new(19, 6.5, 2, TimeSettings, JsonPlayerDecision.Nigiri);
        private static readonly IEqualityComparer<JsonBoard> BoardComparer = new JsonBoardEqualityComparer();

        private class JsonBoardEqualityComparer : IEqualityComparer<JsonBoard>
        {
            public bool Equals(JsonBoard x, JsonBoard y)
            {
                if (x.BlackCaptures != y.BlackCaptures) return false;
                if (x.WhiteCaptures != y.WhiteCaptures) return false;
                if (!x.GameSettings.Equals(y.GameSettings)) return false;
                if (!x.Stones.SequenceEqual(y.Stones)) return false;
                if (x.Next != y.Next) return false;

                return true;
            }

            public int GetHashCode([DisallowNull] JsonBoard obj)
            {
                return obj.GetHashCode();
            }
        }

        [Fact]
        public async Task WhenGameIsCreatedTheGameIdIsReturned()
        {
            var owner = await StartHubConnectionAsync();
            var client = owner.CreateGameClient();

            await owner.CreateGameAsync(GameSettings);
            var gameId = await client.VerifyGameCreated(Times.Once());
            Assert.NotNull(gameId);
        }

        [Fact]
        public async Task WhenGameHasStartedEachPlayerGetsTheInitialBoardPosition()
        {
            var owner = await StartHubConnectionAsync();
            var challenger = await StartHubConnectionAsync();

            var ownerClient = owner.CreateGameClient();
            var challengerClient = challenger.CreateGameClient();

            await owner.CreateGameAsync(GameSettings);
            var gameId = await ownerClient.VerifyGameCreated(Times.Once());

            await challenger.JoinGameAsync(gameId);

            var challengerBoard = await challengerClient.VerifyGameStarted(Times.Once());
            var ownerBoard = await ownerClient.VerifyGameStarted(Times.Once());

            Assert.NotEqual(challengerBoard.AssignedColor, ownerBoard.AssignedColor);
            Assert.Equal(challengerBoard.Board, ownerBoard.Board, BoardComparer);
        }

        [Fact]
        public void HandicapGameMeansThatWhiteIsStartingTheGame()
        {
            var handicap = Enumerable.Range(2, 8).ToArray();
            Assert.All(handicap, async x =>
            {
                var gameSettings = Fixture.GameSettings(handicap: x);

                var owner = await StartHubConnectionAsync();
                var challenger = await StartHubConnectionAsync();

                var ownerClient = owner.CreateGameClient();
                var challengerClient = challenger.CreateGameClient();

                await owner.CreateGameAsync(gameSettings);
                var gameId = await ownerClient.VerifyGameCreated(Times.Once());

                await challenger.JoinGameAsync(gameId);

                var challengerBoard = await challengerClient.VerifyGameStarted(Times.Once());
                var ownerBoard = await ownerClient.VerifyGameStarted(Times.Once());

                Assert.Equal(JsonColor.White, challengerBoard.Board.Next);
                Assert.Equal(JsonColor.White, ownerBoard.Board.Next);
            });
        }

        [Fact]
        public async Task NonAssignedPlayerCanNotPlayTheNextMove()
        {
            var gameSettings = Fixture.GameSettings(playerDecision: JsonPlayerDecision.OwnerTakesBlack);

            var owner = await StartHubConnectionAsync();
            var challenger = await StartHubConnectionAsync();

            var ownerClient = owner.CreateGameClient();
            var challengerClient = challenger.CreateGameClient();

            await owner.CreateGameAsync(gameSettings);
            var gameId = await ownerClient.VerifyGameCreated(Times.Once());

            await challenger.JoinGameAsync(gameId);

            await challengerClient.VerifyGameStarted(Times.Once());
            await ownerClient.VerifyGameStarted(Times.Once());

            await Assert.ThrowsAsync<HubException>(() => challenger.AddMoveAsync(gameId, new JsonPoint(1, 1)));
        }

        [Fact]
        public async Task CanNotPlayAMoveOnAnOccupiedPoint()
        {
            var owner = await StartHubConnectionAsync();
            var ownerClient = owner.CreateGameClient();
            var gameId = await Fixture.CreateGameAsync(owner, ownerClient, Fixture.GameSettings(playerDecision: JsonPlayerDecision.OwnerTakesBlack));

            var challenger = await StartHubConnectionAsync();
            var challengerClient = challenger.CreateGameClient();

            await challenger.JoinGameAsync(gameId);

            var challengerPos = await challengerClient.VerifyGameStarted(Times.Once());
            var ownerPos = await ownerClient.VerifyGameStarted(Times.Once());

            var point = new JsonPoint(4, 4);

            await owner.AddMoveAsync(gameId, point);
            await challengerClient.VerifyBoardUpdated(Times.Once());
            await ownerClient.VerifyBoardUpdated(Times.Once());

            await Assert.ThrowsAsync<HubException>(() => challenger.AddMoveAsync(gameId, point));
        }

        [Fact]
        public async Task CapturingAStoneIncreasesCaptureCount()
        {
            var owner = await StartHubConnectionAsync();
            var ownerClient = owner.CreateGameClient();
            var gameId = await Fixture.CreateGameAsync(owner, ownerClient, Fixture.GameSettings(playerDecision: JsonPlayerDecision.OwnerTakesBlack));
            
            var challenger = await StartHubConnectionAsync();
            var challengerClient = challenger.CreateGameClient();

            await challenger.JoinGameAsync(gameId);

            await challengerClient.VerifyGameStarted(Times.Once());
            await ownerClient.VerifyGameStarted(Times.Once());

            async Task<JsonBoard> AddMove(HubConnection connection, JsonPoint point)
            {
                await connection.AddMoveAsync(gameId, point);
                await ownerClient.VerifyBoardUpdated(Times.Once());
                return await challengerClient.VerifyBoardUpdated(Times.Once());
            }

            async Task Pass()
            {
                await owner.PassAsync(gameId);
                Assert.Equal(JsonColor.Black, await ownerClient.VerifyPlayerPassedAsync(Times.Once()));
                Assert.Equal(JsonColor.Black, await challengerClient.VerifyPlayerPassedAsync(Times.Once()));
            }

            await AddMove(owner, new (4, 4));
            await AddMove(challenger, new (4, 3));
            await Pass();
            await AddMove(challenger, new (3, 4));
            await Pass();
            await AddMove(challenger, new (5, 4));
            await Pass();
            var board = await AddMove(challenger, new (4, 5));

            Assert.Equal(1, board.WhiteCaptures);
            Assert.DoesNotContain(new JsonStone(JsonColor.Black, new JsonPoint(4, 4)), board.Stones);

            await Pass();
            var board2 = await AddMove(challenger, new (4, 4));
            Assert.Contains(new JsonStone(JsonColor.White, new JsonPoint(4, 4)), board2.Stones);
        }

        [Fact]
        public async Task AddingAMoveTriggersABoardUpdate()
        {
            var gameSettings = Fixture.GameSettings(playerDecision: JsonPlayerDecision.OwnerTakesBlack);

            var owner = await StartHubConnectionAsync();
            var challenger = await StartHubConnectionAsync();

            var ownerClient = owner.CreateGameClient();
            var challengerClient = challenger.CreateGameClient();

            await owner.CreateGameAsync(gameSettings);
            var gameId = await ownerClient.VerifyGameCreated(Times.Once());

            await challenger.JoinGameAsync(gameId);

            var challengerInitialPos = await challengerClient.VerifyGameStarted(Times.Once());
            var ownerInitialPos = await ownerClient.VerifyGameStarted(Times.Once());

            var point = new JsonPoint(1, 1);
            await owner.AddMoveAsync(gameId, point);

            var challengerBoard = await challengerClient.VerifyBoardUpdated(Times.Once());
            var ownerBoard = await ownerClient.VerifyBoardUpdated(Times.Once());

            Assert.Equal(challengerBoard, ownerBoard, BoardComparer);
            Assert.NotEqual(challengerBoard, challengerInitialPos.Board, BoardComparer);

            var expectedAddedStone = new JsonStone(ownerInitialPos.AssignedColor, point);
            Assert.Equal(expectedAddedStone, challengerBoard.Stones.Last());
        }
    }
}
