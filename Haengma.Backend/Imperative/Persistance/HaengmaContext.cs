using Haengma.Backend.Imperative.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace Haengma.Backend.Imperative.Persistance
{
    public class HaengmaContext : DbContext
    {
        public HaengmaContext() { }
        public HaengmaContext(DbContextOptions<HaengmaContext> options) : base(options)
        {
        }

        private static ValueConverter<T, Guid> IdConverter<T>(Expression<Func<Guid, T>> convertFrom) where T : Id => new(
            id => id.Value,
            convertFrom);

        public DbSet<User>? Users { get; set; }
        public DbSet<Game>? Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.Property(x => x.Id).HasConversion(IdConverter(id => new UserId(id)));
                b.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Game>(b =>
            {
                b.Property(x => x.Id).HasConversion(IdConverter(id => new GameId(id))).IsRequired();
                b.HasKey(x => x.Id);
                b.Property(x => x.Black).HasConversion(IdConverter(id => new UserId(id))).IsRequired();
                b.Property(x => x.White).HasConversion(IdConverter(id => new UserId(id))).IsRequired();
                b.HasOne<User>().WithMany().HasForeignKey(x => x.Black);
                b.HasOne<User>().WithMany().HasForeignKey(x => x.White);
            });
        }
    }
}
