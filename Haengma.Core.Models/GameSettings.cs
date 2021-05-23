namespace Haengma.Core.Models
{
    public enum ColorDecision { Nigiri, OwnerTakesBlack, ChallengerTakesBlack }

    public record GameSettings(
        int BoardSize,
        double Komi,
        int Handicap,
        TimeSettings TimeSettings,
        ColorDecision ColorDecision
    );
}
