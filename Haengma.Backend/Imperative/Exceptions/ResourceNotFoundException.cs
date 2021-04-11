using Haengma.Backend.Imperative.Models;
using System;

namespace Haengma.Backend.Imperative.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(Id resourceId) : base($"Resource with ID {resourceId.AsString} could not be found.")
        {
        }
    }
}
