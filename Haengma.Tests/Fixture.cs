using Haengma.GS.Models;

namespace Haengma.Tests
{
    public static class Fixture
    {
        public static JsonUser CreateUser(string name = "TestUser", int rank = 30, JsonRankType rankType = JsonRankType.Kyu) => new(name, new JsonRank(rank, rankType));
        public static JsonTimeSettings CreateTimeSettings(
            JsonTimeSettingType type = JsonTimeSettingType.ByoYomi,
            int mainTimeInSeconds = 60 * 10,
            int byoYomiPeriods = 5,
            int byoYomiSeconds = 30) => new(type, mainTimeInSeconds, byoYomiPeriods, byoYomiSeconds);

        public static JsonGameSettings CreateGame(
            JsonTimeSettings timeSettings = null,
            int boardSize = 19,
            double komi = 6.5,
            int handicap = 0,
            JsonPlayerDecision playerDecision = JsonPlayerDecision.Nigiri
        ) => new(
            boardSize,
            komi,
            handicap,
            timeSettings ?? CreateTimeSettings(),
            playerDecision
        );
    }
}
