using Haengma.Backend.Imperative.Exceptions;
using Haengma.Backend.Imperative.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Haengma.Backend.Utils
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

        public static T GetOrThrowResourceNotFound<T, TId>(this IQueryable<T> query, TId id) 
            where T : IDEntity<TId>
            where TId : Id => query.SingleOrThrow(x => x.Id == id, () => new ResourceNotFoundException(id));
    }
}
