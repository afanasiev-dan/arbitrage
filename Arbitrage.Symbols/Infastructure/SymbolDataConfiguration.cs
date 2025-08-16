using Arbitrage.Symbols.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arbitrage.Symbols.Infastructure
{
    public class SymbolDataConfiguration
    {
        public void Configure(EntityTypeBuilder<Coin> builder)
        {
            builder.ToTable("Symbols");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired();
            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}