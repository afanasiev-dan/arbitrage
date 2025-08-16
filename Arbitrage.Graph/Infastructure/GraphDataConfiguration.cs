// using Arbitrage.Graph.Domain;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace Arbitrage.Graph.Infastructure
// {
//     public class GraphDataConfiguration
//     {
//        public void Configure(EntityTypeBuilder<Candle> builder)
//         {
//             builder.ToTable("Candle");
//             builder.HasKey(s => s.Id);
//             builder.Property(s => s.Ticker).IsRequired().HasMaxLength(20);
//             builder.HasIndex(s => s.Ticker).IsUnique();
//         }
//     }
// }