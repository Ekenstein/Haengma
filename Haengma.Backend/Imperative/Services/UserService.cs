using Haengma.Backend.Imperative.Models;
using Haengma.Backend.Imperative.Persistance.Repositories;
using System.Collections.Generic;

namespace Haengma.Backend.Imperative.Services
{
    public static class UserService
    {
        public static IReadOnlyList<User> GetUsers(this ServiceContext serviceContext) => serviceContext
            .Transactions
            .Read(t => t.GetUsers());
    }
}
