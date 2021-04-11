using Haengma.Backend.Imperative.Models;

namespace Haengma.Backend.Imperative.Services.Models
{
    public record GameOptions(
        int BoardSize, 
        int Handicap, 
        double Komi,
        UserId Black,
        UserId White);
}
