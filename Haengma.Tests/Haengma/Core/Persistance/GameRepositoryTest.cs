using Haengma.Core.Persistence;
using System.Threading.Tasks;
using Xunit;
using static Xunit.Assert;

namespace Haengma.Tests.Haengma.Core.Persistance
{
    public class GameRepositoryTest : DatabaseTestBase
    {
        [Fact]
        public async Task AddGame_EquivalentTo_GetGame()
        {
            var game = Fixture.Game();
            await Transactions.WriteAsync(tx => tx.AddGame(game));
            var actualGame = await Transactions.ReadAsync(tx => tx.GetGameByIdAsync(game.Id));

            Equal(game, actualGame);
        }

        [Fact]
        public async Task GetGameByIdAsync_NoGameWithId_NoSuchEntityException()
        {
            await ThrowsAsync<NoSuchEntityException>(() => Transactions.ReadAsync(tx => tx.GetGameByIdAsync(new(Fixture.RandomIdString()))));
        }

        [Fact]
        public async Task UpdateSgfAsync_NoGameWithId_NoSuchEntityException()
        {
            await ThrowsAsync<NoSuchEntityException>(() => Transactions.WriteAsync(tx => tx.UpdateSgfAsync(new(Fixture.RandomIdString()), "apa")));
        }

        [Fact]
        public async Task UpdateSgfAsync_SgfEquivalentTo_GetGame_Sgf()
        {
            const string sgf = "apa";
            var game = Fixture.Game();
            await Transactions.WriteAsync(tx => tx.AddGame(game));
            await Transactions.WriteAsync(tx => tx.UpdateSgfAsync(game.Id, sgf));
            var actualGame = await Transactions.ReadAsync(tx => tx.GetGameByIdAsync(game.Id));

            Equal(sgf, actualGame.Sgf);
        }
    }
}
