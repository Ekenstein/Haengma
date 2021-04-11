using Haengma.Backend.Imperative.Models;
using Haengma.Backend.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Backend.Imperative.Persistance.Repositories
{
    public static class UserRepository
    {
        public static User GetUserById(this IReadOnlyTransaction transaction, UserId id) => transaction
            .Query<User>()
            .GetOrThrowResourceNotFound(id);

        public static IReadOnlyList<User> GetUsers(this IReadOnlyTransaction transaction) => transaction
            .Query<User>()
            .ToArray();
    }
}
