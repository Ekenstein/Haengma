using System;

namespace Haengma.Backend.Imperative.Models
{
    public record GameId(Guid Id) : Id(Id);
    public record Game(
        GameId Id,
        UserId Black,
        UserId White,
        string Sgf
    ) : IDEntity<GameId>;

    public record GameInfo(
        int BoardSize,
        double Komi,
        int Handicap,
        UserId BlackUser,
        UserId WhiteUser,
        string Sgf);
}
