using Haengma.Core.Models;
using Haengma.Core.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Haengma.Core.Persistence
{
    public class HaengmaContext : DbContext
    {
        private static readonly ValueConverter<UserId, string> UserIdConverter = new(
            x => x.Value,
            x => new UserId(x)
        );

        private static readonly ValueConverter<GameId, string> GameIdConverter = new(
            x => x.Value,
            x => new GameId(x)
        );

        public HaengmaContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InternalGame>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasConversion(GameIdConverter);
                b.Property(x => x.BlackPlayer).HasConversion(UserIdConverter);
                b.Property(x => x.WhitePlayer).HasConversion(UserIdConverter);
                b.Property(x => x.ColorDecision).HasConversion(new EnumToStringConverter<ColorDecision>());
                b.Property(x => x.TimeSettingsType).HasConversion(new EnumToStringConverter<TimeSettingsType>());
            });
        }
    }
}
