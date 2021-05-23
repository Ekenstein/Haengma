using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Haengma.Core.Persistence
{
    public class TransactionFactory : ITransactionFactory
    {
        private readonly Func<DbContext> _provider;

        public TransactionFactory(Func<DbContext> provider)
        {
            _provider = provider;
        }

        public async Task<T> WriteAsync<T>(Func<ITransaction, Task<T>> block)
        {
            using var context = _provider();
            var result = await block(new Transaction(context));
            await context.SaveChangesAsync();
            return result;
        }

        public async Task<T> ReadAsync<T>(Func<IReadOnlyTransaction, Task<T>> block)
        {
            using var context = _provider();
            return await block(new Transaction(context));
        }

        private class Transaction : ITransaction
        {
            private readonly DbContext _context;

            public Transaction(DbContext context)
            {
                _context = context;
            }

            public void Add<T>(T entity) => _context.Add(entity);

            public IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsNoTracking();

            public void Remove<T>(T entity) => _context.Remove(entity);

            public void Update<T>(T entity) => _context.Update(entity);
        }
    }

    public interface ITransactionFactory
    {
        Task<T> WriteAsync<T>(Func<ITransaction, Task<T>> block);
        Task<T> ReadAsync<T>(Func<IReadOnlyTransaction, Task<T>> block);
    }

    public interface ITransaction : IReadOnlyTransaction
    {
        void Remove<T>(T entity);
        void Update<T>(T entity);
        void Add<T>(T entity);
    }

    public interface IReadOnlyTransaction
    {
        IQueryable<T> Query<T>() where T : class;
    }
}
