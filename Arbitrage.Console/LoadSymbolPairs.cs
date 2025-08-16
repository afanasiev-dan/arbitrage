using System.Text.Json;
using System.Text.Json.Serialization;
using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.Exchange.Infastructure.Repositories;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.Symbols.Infastructure.Repositories;
using Arbitrage.WebApi.Infastructure;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Test
{
    public class LoadSymbolPairs
    {
        public static async Task Execute()
        {
            var jsonPath = "/home/daniil/projects/git/ArbitrageCHO/BackEnd/Arbitrage.Console/data/marketPairs.json";
            string dbPath = "/home/daniil/projects/git/ArbitrageCHO/BackEnd/Arbitrage.WebApi/arbitrage.db";

            FilesValidation(jsonPath, dbPath);

            var json = await File.ReadAllTextAsync(jsonPath);
            var data = JsonSerializer.Deserialize<List<MarketPairDto>>(json);

            if (data.Count <= 0)
                throw new NullReferenceException("Нет объектов для добавления в базу");

            // ShowDataInConsole(data);

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            using var context = new AppDbContext(optionsBuilder.Options);

            ICoinRepository symbolRepository = new CoinRepository(context);
            IExchangeRepository exchangeRepository = new ExchangeRepository(context);
            ICurrencyPairRepository currencyPairRepository = new CurrencyPairRepository(context);

            var symbols = await symbolRepository.GetAllAsync();
            var exchanges = await exchangeRepository.GetAllAsync();
            var pairs = await currencyPairRepository.GetAllAsync();


            await AddDataToDb(context, data, symbols.ToList(), exchanges, pairs.ToList());

            async Task AddDataToDb(AppDbContext appDbContext,
                                List<MarketPairDto> pairsLoads,
                                List<Coin> symbols,
                                List<ExchangeModel> exchanges,
                                List<CurrencyPair> pairs)
            {
                var pairsToAdd = new List<CurrencyPair>();

                foreach (var x in pairsLoads)
                {
                    var baseSymbol = symbols.FirstOrDefault(s => s.Name.ToUpper() == x.Base.ToUpper());
                    var quoteSymbol = symbols.FirstOrDefault(s => s.Name.ToUpper() == x.Quote.ToUpper());
                    var exchange = exchanges.FirstOrDefault(e => e.Name.ToUpper() == x.Exchange.ToUpper());

                    if (baseSymbol == null || quoteSymbol == null || exchange == null)
                    {
                        Console.WriteLine($"Пропущена пара {x.Ticker}: не найдены связанные данные. Для биржи {x.Exchange}, для монет {x.Base} и {x.Quote}, с типом {x.MarketType}");
                        continue;
                    }



                    // Проверяем, существует ли уже такая пара в БД или в списке на добавление
                    bool existsInDb = pairs.Any(p =>
                        p.Pair.ToUpper() == x.Ticker.ToUpper() &&
                        p.ExchangeId == exchange.Id &&
                        p.MarketType == x.MarketType);

                    bool existsInList = pairsToAdd.Any(p =>
                        p.Pair.ToUpper() == x.Ticker.ToUpper() &&
                        p.ExchangeId == exchange.Id &&
                        p.MarketType == x.MarketType);

                    if (!existsInDb && !existsInList)
                    {
                        pairsToAdd.Add(new CurrencyPair()
                        {
                            Id = Guid.NewGuid(),
                            Pair = x.Ticker,
                            BaseCoinId = baseSymbol.Id,
                            QuoteCoinId = quoteSymbol.Id,
                            ExchangeId = exchange.Id,
                            MarketType = x.MarketType
                        });
                    }
                }

                await appDbContext.CurrencyPairs.AddRangeAsync(pairsToAdd);
                await appDbContext.SaveChangesAsync();

                Console.WriteLine($"Данные загружены в базу. Добавлено {pairsToAdd.Count} из {pairsLoads.Count} пар.");
            }

            void ShowDataInConsole(List<CurrencyPair> list)
            {
                System.Console.WriteLine($"Objects count {data.Count()}");

                foreach (var asset in list)
                    System.Console.WriteLine(asset.Pair);
            }

            void FilesValidation(string s, string dbPath1)
            {
                if (!File.Exists(s))
                    throw new NullReferenceException("Такой файл не существует");

                if (!File.Exists(dbPath1))
                    throw new NullReferenceException("База данных не существует");
            }
        }
    }

    internal class MarketPairDto
    {
        [JsonPropertyName("original")]
        public string Original { get; set; }

        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("quote")]
        public string Quote { get; set; }

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }

        [JsonPropertyName("typeMarket")]
        public MarketType MarketType { get; set; }

    }
}