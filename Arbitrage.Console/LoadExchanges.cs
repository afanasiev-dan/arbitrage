using System.Text.Json;
using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.ExchangeDomain;
using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.WebApi.Infastructure;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Test
{
    public class LoadExchanges
    {

        public static async Task Execute()
        {
            string dbPath = "/home/daniil/projects/git/ArbitrageCHO/BackEnd/Arbitrage.WebApi/arbitrage.db";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            using var context = new AppDbContext(optionsBuilder.Options);

            await AddDataToDb(context);

            async Task AddDataToDb(AppDbContext appDbContext)
            {
                List<string> exchangesNameToAdd = [
                    Exchanges.ByBit,
                    Exchanges.KuCoin,
                    Exchanges.Gate,
                    Exchanges.Mexc,
                    Exchanges.Htx,
                    Exchanges.LBank,
                    Exchanges.Binance
                ];

                List<ExchangeModel> newExchangesToAdd = new();

                foreach (var exchangeName in exchangesNameToAdd)
                {
                    var existingExchange = await appDbContext.Exchanges.FirstOrDefaultAsync(e => e.Name == exchangeName);

                    if (existingExchange == null)
                    {
                        ExchangeModel newExchange = new ExchangeModel()
                        {
                            Id = Guid.NewGuid(),
                            Name = exchangeName
                        };

                        newExchangesToAdd.Add(newExchange);
                    }
                }

                if (newExchangesToAdd.Any())

                await appDbContext.Exchanges.AddRangeAsync(newExchangesToAdd);
                await appDbContext.SaveChangesAsync();

                Console.WriteLine($"Данные загружены в базу. Добавлено {newExchangesToAdd.Count} новых бирж.");
            }

        }
    }
}