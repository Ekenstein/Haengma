using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haengma.Core.Logics.Games
{
    public interface IGameClock
    {
        event EventHandler? TimeEnded;
    }
}
