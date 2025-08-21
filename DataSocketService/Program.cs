using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using DataSocketService.Other;
using DataSocketService.Utils;

public class Program
{
    static List<ExchangeBase> Exchanges = new();

    public static async Task Main(string[] args)
    {
        var currencyPair = await PairF.GetCurrency();
        var arbitragePair = PairF.GetArbitrage(currencyPair);

        await CreateConnect(currencyPair);
        await StartListen();
        _ = Task.Run(SendInfo);

        Console.ReadKey();
    }

    static async Task CreateConnect(List<CurrencyPairResponceDto> currencyPairs)
    {
        var byExchange = currencyPairs.GroupBy(p => new { p.ExchangeName, p.MarketType }).ToList();

        foreach (var group in byExchange)
        {
            var currencyPair = group.ToList();
            var exchange = ExchangeFactory.Create(group.Key.ExchangeName, group.Key.MarketType, currencyPair);
            Exchanges.Add(exchange);
        }
    }

    static async Task StartListen()
    {
        var tasks = Exchanges.Select(exchange => exchange.ConnectBook());
        await Task.WhenAll(tasks);
    }

    static async Task SendInfo()
    {
        while (true)
        {
            await Task.Delay(2000);

            var allBooks = Exchanges.SelectMany(x => x.GetAllBooks()).ToList();
            foreach (var item in allBooks)
            {
                string name = FormatUtils.ExchangeToStr(item.Info.ExchangeName, item.Info.MarketType);
                Console.WriteLine($"[{name}] {item.Book.Asks[0].price} {item.Book.Bids[0].price}");
            }
        }
    }
}
