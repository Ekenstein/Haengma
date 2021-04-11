using Microsoft.EntityFrameworkCore;
using Pidgin;
using System;
using System.Linq;

namespace Haengma.Backend.Imperative.Persistance
{
    public interface IReadOnlyTransaction
    {
        IQueryable<T> Query<T>() where T : class;
    }

    public interface IWritableTransaction : IReadOnlyTransaction
    {
        void Add<T>(T t);
        void Remove<T>(T t);
        void Update<T>(T t);
        void Commit();
    }

    public interface ITransactionFactory
    {
        T Read<T>(Func<IReadOnlyTransaction, T> transaction);
        void Read(Action<IReadOnlyTransaction> transaction);
        T Write<T>(Func<IWritableTransaction, T> transaction);
        void Write(Action<IWritableTransaction> transaction);
    }

    public class TransactionFactory<TContext> : ITransactionFactory where TContext : DbContext
    {
        private readonly Func<TContext> _provider;

        public TransactionFactory(Func<TContext> provider)
        {
            _provider = provider;
        }

        public T Read<T>(Func<IReadOnlyTransaction, T> transaction)
        {
            using var context = _provider();
            return transaction(new ReadOnlyTransaction(context));
        }

        public void Read(Action<IReadOnlyTransaction> transaction)
        {
            Read(t =>
            {
                transaction(t);
                return Unit.Value;
            });
        }

        public T Write<T>(Func<IWritableTransaction, T> transaction)
        {
            using var context = _provider();
            return transaction(new WritableTransaction(context));
        }

        public void Write(Action<IWritableTransaction> transaction)
        {
            Write(t =>
            {
                transaction(t);
                return Unit.Value;
            });
        }

        private class WritableTransaction : IWritableTransaction
        {
            private readonly TContext _context;

            public WritableTransaction(TContext context)
            {
                _context = context;
            }

            public void Add<T>(T t) => _context.Add(t);

            public void Commit() => _context.SaveChanges();

            public IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsNoTracking();

            public void Remove<T>(T t) => _context.Remove(t);

            public void Update<T>(T t) => _context.Update(t);
        }

        private class ReadOnlyTransaction : IReadOnlyTransaction
        {
            private readonly TContext _context;

            public ReadOnlyTransaction(TContext context)
            {
                _context = context;
            }

            public IQueryable<T> Query<T>() where T : class => _context.Set<T>().AsNoTracking();
        }
    }
}
