using Haengma.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haengma.Core.Persistence.Models
{
    internal enum TimeSettingsType { ByoYomi }

    [Table("Games")]
    internal record InternalGame(
        GameId Id,
        UserId BlackPlayer,
        UserId WhitePlayer,
        int BoardSize,
        double Komi,
        int Handicap,
        TimeSettingsType TimeSettingsType,
        int MainTimeInSeconds,
        int ByoYomiPeriods,
        int ByoYomiSeconds,
        string Sgf,
        ColorDecision ColorDecision
    );
}
