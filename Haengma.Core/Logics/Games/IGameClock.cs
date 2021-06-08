using System;

namespace Haengma.Core.Logics.Games
{
    public interface IGameClock
    {
        event EventHandler? TimeEnded;
    }
}
