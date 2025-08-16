using System.Text.Json;
using System.Text.Json.Serialization;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.Symbols.Infastructure.Repositories;
using Arbitrage.WebApi.Infastructure;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Test;

public class LoadSymbols
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
        var symbols = await symbolRepository.GetAllAsync();

        await AddDataToDb(context, data, symbols.ToList());

        async Task AddDataToDb(AppDbContext appDbContext,
                            List<MarketPairDto> pairsLoads,
                            List<Coin> coins)
        {
            var coinsToAdd = new List<Coin>();
            var uniqOriginCoinsToAdd = pairsLoads
                .GroupBy(s => s.Original.ToUpper()) // нормализуем к верхнему регистру сразу
                .Select(g => g.First().Original.ToUpper())
                .ToList();

            var uniqQouteCoinsToAdd = pairsLoads
                .GroupBy(s => s.Quote.ToUpper()) // нормализуем к верхнему регистру сразу
                .Select(g => g.First().Quote.ToUpper())
                .ToList();

            // Объединяем и убираем дубликаты
            var allUniqueTickers = uniqOriginCoinsToAdd
                .Concat(uniqQouteCoinsToAdd)
                .Distinct()
                .ToList();

            // Получаем существующие тикеры из базы (в верхнем регистре)
            var existingTickers = await appDbContext.Coins
                .Select(s => s.Name.ToUpper())
                .ToListAsync();

            List<Coin> newCoinsToAdd = new();

            foreach (var ticker in allUniqueTickers)
            {
                if (existingTickers.Contains(ticker.ToUpper()))
                    continue;

                Coin coin = new Coin()
                {
                    Id = Guid.NewGuid(),
                    Name = ticker.ToUpper() // сохраняем в едином формате
                };

                newCoinsToAdd.Add(coin);
            }

            if (newCoinsToAdd.Any())
            {
                await appDbContext.Coins.AddRangeAsync(newCoinsToAdd);
                await appDbContext.SaveChangesAsync();
            }

            Console.WriteLine($"Данные загружены в базу. Добавлено {newCoinsToAdd.Count} новых тикеров.");
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

    public class SymbolLoad
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }
    }
}

