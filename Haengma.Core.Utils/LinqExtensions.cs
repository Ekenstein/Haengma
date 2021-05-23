using System;
using System.Linq;
using System.Linq.Expressions;

namespace Haengma.Core.Utils
{
    public static class LinqExtensions
    {
        public static T SingleOrThrow<T>(this IQueryable<T> query, Func<Exception> ex)
        {
            var items = query.Take(2).ToArray();
            if (items.Length <= 0 || items.Length > 1)
            {
                throw ex();
            }

            return items[0];
        }

        public static T SingleOrThrow<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, Func<Exception> ex) => query
            .Where(predicate)
            .SingleOrThrow(ex);
    }
}
