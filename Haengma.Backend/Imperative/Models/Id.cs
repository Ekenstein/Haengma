using System;

namespace Haengma.Backend.Imperative.Models
{
    public abstract record Id(Guid Value)
    {
        public string AsString => Value.ToString();
    }

    public interface IDEntity<TId> where TId : Id
    {
        TId Id { get; }
    }
}
