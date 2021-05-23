using System.Collections.Generic;

namespace Haengma.GS.Models
{
    public record JsonStone(
        JsonColor Color,
        JsonPoint Point
    );

    public record JsonBoard(
        int WhiteCaptures,
        int BlackCaptures,
        JsonGameSettings GameSettings,
        IEnumerable<JsonStone> Stones,
        JsonColor Next
    );
}
