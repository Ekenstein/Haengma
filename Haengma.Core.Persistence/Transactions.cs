using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<T> WriteAndReturnAsync<T>(Func<ITransaction, Task<T>> block)
        {
            using var context = _provider();
            var result = await block(new Transaction(context)).ConfigureAwait(false);
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

            public void AddRange<T>(IEnumerable<T> entities) => _context.AddRange(entities);
            public IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsNoTracking();

            public void Remove<T>(T entity) => _context.Remove(entity);

            public void RemoveRange<T>(IEnumerable<T> entities) => _context.RemoveRange(entities);

            public void Update<T>(T entity) => _context.Update(entity);

            public void UpdateRange<T>(IEnumerable<T> entities) => _context.UpdateRange(entities);
        }
    }

    public interface ITransactionFactory
    {
        Task<T> WriteAndReturnAsync<T>(Func<ITransaction, Task<T>> block);
        public Task WriteAsync(Func<ITransaction, Task> block) => WriteAndReturnAsync(async tx =>
        {
            await block(tx);
            return Task.FromResult(1);
        });
        Task<T> ReadAsync<T>(Func<IReadOnlyTransaction, Task<T>> block);
    }

    public static class TransactionFactoryExtensions
    {
        public static async Task WriteAsync(this ITransactionFactory factory, Action<ITransaction> block) => await factory
            .WriteAsync(tx =>
            {
                block(tx);
                return Task.CompletedTask;
            });

        public static async Task<T> WriteAndReturnAsync<T>(this ITransactionFactory factory, Func<ITransaction, T> block) => await factory
            .WriteAndReturnAsync(tx => Task.FromResult(block(tx)));
    }

    public interface ITransaction : IReadOnlyTransaction
    {
        void Remove<T>(T entity);
        void RemoveRange<T>(IEnumerable<T> entities);
        void Update<T>(T entity);
        void Add<T>(T entity);
        void AddRange<T>(IEnumerable<T> entities);
        void UpdateRange<T>(IEnumerable<T> entities);
    }

    public interface IReadOnlyTransaction
    {
        IQueryable<T> Query<T>() where T : class;
    }
}
