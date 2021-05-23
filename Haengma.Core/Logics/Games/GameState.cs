using Haengma.Core.Models;
using System.Collections.Generic;

namespace Haengma.Core.Logics.Games
{
    public record GameState(IReadOnlyDictionary<UserId, Color> Players);
}
