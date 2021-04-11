using System;

namespace Haengma.GS.Models
{
    public record JsonGame(
        int BoardSize, 
        double Komi, 
        int Handicap, 
        Guid BlackUserId,
        Guid WhiteUserId,
        string Sgf
    );
}
