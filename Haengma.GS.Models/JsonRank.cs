namespace Haengma.GS.Models
{
    public enum JsonRankType { Kyu, Dan }

    public record JsonRank(int Rank, JsonRankType RankType);
}
