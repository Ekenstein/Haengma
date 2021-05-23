namespace Haengma.Core.Models
{
    public record GameId(string Value);

    public record Game(
        GameId Id,
        UserId BlackPlayer,
        UserId WhitePlayer,
        string Sgf,
        GameSettings GameSettings
    );
}
