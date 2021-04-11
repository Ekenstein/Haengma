using System;

namespace Haengma.GS.Models
{
    public record JsonPostGameOptions(
        int BoardSize,
        double Komi,
        int Handicap,
        Guid BlackUserId,
        Guid WhiteUserId
    );
}
