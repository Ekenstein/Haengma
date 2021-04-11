using System;

namespace Haengma.Backend.Imperative.Models
{
    public record UserId(Guid Id) : Id(Id);
    public record User(UserId Id, string Name) : IDEntity<UserId>;
}
