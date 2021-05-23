using Haengma.Core.Models;
using System;

namespace Haengma.Core.Persistence
{
    public class NoSuchEntityException : Exception
    {
        public NoSuchEntityException(GameId gameId) : base($"No game was found with the id {gameId.Value}.") { }
    }
}
