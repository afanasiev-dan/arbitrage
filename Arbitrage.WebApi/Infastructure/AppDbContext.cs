using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using Arbitrage.Graph.Domain;
using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.Scaner.Domain.Entities;
using Arbitrage.User.Domain.Entities;
using Arbitrage.Notification.Domain.Entities;
using Arbitrage.Domain.TelegramBot.Entities;

namespace Arbitrage.WebApi.Infastructure;

public class AppDbContext : DbContext
{
    public DbSet<Candle> Candles { get; set; }
    public DbSet<Coin> Coins { get; set; }
    public DbSet<CurrencyPair> CurrencyPairs { get; set; }
    public DbSet<ExchangeModel> Exchanges { get; set; }
    public DbSet<ScanerModel> ScanerData { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<NotificationModel> Notifications { get; set; }
    public DbSet<TelegramUserSettings>  TelegramUserSettings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        System.Console.WriteLine($"Я бдшка {DateTime.Now}");
     }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Coin).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Candle).Assembly);

        modelBuilder.Entity<Candle>()
            .HasOne(e => e.Exchange)
            .WithMany()
            .HasForeignKey(e => e.ExchangeId);

        modelBuilder.Entity<Candle>()
            .HasOne(e => e.Pair)
            .WithMany()
            .HasForeignKey(e => e.CurrencyPairId);

        modelBuilder.Entity<ScanerModel>()
            .HasOne(e => e.TickerLong)
            .WithMany()
            .HasForeignKey(s => s.TickerLongId);

        modelBuilder.Entity<ScanerModel>()
            .HasOne(e => e.TickerShort)
            .WithMany()
            .HasForeignKey(s => s.TickerShortId);

        modelBuilder.Entity<ScanerModel>()
            .HasOne(e => e.BaseCoin)
            .WithMany()
            .HasForeignKey(s => s.BaseCoinId);

        modelBuilder.Entity<ScanerModel>()
            .HasOne(e => e.QuoteCoin)
            .WithMany()
            .HasForeignKey(s => s.QuoteCoinId);

        modelBuilder.Entity<ScanerModel>()
            .HasOne(e => e.ExchangeLong)
            .WithMany()
            .HasForeignKey(s => s.ExchangeIdLong);

        modelBuilder.Entity<ScanerModel>()
            .HasOne(e => e.ExchangeShort)
            .WithMany()
            .HasForeignKey(s => s.ExchangeIdShort);

        // modelBuilder.Entity<CurrencyPair>()
        //     .HasOne(cp => cp.BaseCoin)
        //     .WithMany()
        //     .HasForeignKey(cp => cp.BaseCoinId);

        // modelBuilder.Entity<CurrencyPair>()
        //     .HasOne(cp => cp.QuoteCoin)
        //     .WithMany()
        //     .HasForeignKey(cp => cp.QuoteCoinId);

        // Для SQLite нужно преобразование DateTime
        // SQLite не имеет встроенного типа для DateTime и хранит их как TEXT, REAL или INTEGER.
        // Этот код настраивает EF Core для сохранения всех свойств DateTime и DateTime?
        // как строки в формате ISO 8601 (UTC). Это надежный и рекомендуемый подход.
        if (Database.IsSqlite())
        {
            var dateTimeConverter = new ValueConverter<DateTime, string>(
                v => v.ToUniversalTime().ToString("o"),
                v => DateTime.Parse(v, null, System.Globalization.DateTimeStyles.RoundtripKind));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, string>(
                v => v.HasValue ? v.Value.ToUniversalTime().ToString("o") : null,
                v => string.IsNullOrEmpty(v) ? null : (DateTime?)DateTime.Parse(v, null, System.Globalization.DateTimeStyles.RoundtripKind));

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
            }
        }
    }
}