using Haengma.Core.Persistence;
using Haengma.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;

namespace Haengma.Tests
{
    public abstract class DatabaseTestBase
    {
        private static DbContext CreateDbContext() => new DbContextOptionsBuilder()
            .UseSqlServer(
                "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Haengma;Integrated Security=SSPI;",
                options => options.MigrationsAssembly("Haengma.GS")
            )
            .Let(x => new HaengmaContext(x.Options));

        private static readonly Lazy<ITransactionFactory> lazyTransactionFactory = new (() => {
            return new TransactionFactory(CreateDbContext);
        });

        public static ITransactionFactory Transactions => lazyTransactionFactory.Value;
    }
}
