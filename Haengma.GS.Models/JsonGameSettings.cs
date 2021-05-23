namespace Haengma.GS.Models
{
    public enum JsonPlayerDecision { Nigiri, OwnerTakesBlack, ChallengerTakesBlack }

    public record JsonGameSettings(
        int BoardSize,
        double Komi,
        int Handicap,
        JsonTimeSettings TimeSettings,
        JsonPlayerDecision ColorDecision
    );
}
