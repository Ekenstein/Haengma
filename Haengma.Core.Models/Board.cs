using System.Collections.Generic;

namespace Haengma.Core.Models
{
    public record Point(int X, int Y);
    public enum Color { Black, White }

    public record Stone(
        Point Point,
        Color Color
    );

    public record Board(
        GameSettings GameSettings,
        int WhiteCaptures,
        int BlackCaptures,
        IEnumerable<Stone> Stones,
        Color Next
    );
}
