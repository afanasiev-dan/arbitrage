using Arbitrage.ExchangeDomain;
using Arbitrage.Scaner.Presentation.Dto;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using DataSocketService.Model;
using DataSocketService.Other;
using DataSocketService.Utils;
using Newtonsoft.Json;

public class Program
{
    static List<ExchangeBase> Exchanges = new();
    static List<ArbitragePair> ArbitragePair = new();

    public static async Task Main(string[] args)
    {
        var currencyPair = await PairF.GetCurrency();

        await CreateConnect(currencyPair);

        var currencyPairBook = Exchanges.SelectMany(x => x.GetAllBooks()).ToList();
        ArbitragePair = PairF.GetArbitrage(currencyPairBook);
        Console.WriteLine($"create {ArbitragePair.Count} Arb.pair");

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

            //var allBooks = Exchanges.SelectMany(x => x.GetAllBooks()).ToList();
            //foreach (var item in allBooks)
            //{
            //    string name = FormatUtils.ExchangeToStr(item.Info.ExchangeName, item.Info.MarketType);
            //    Console.WriteLine($"[{name}] {item.Book.Asks[0].price} {item.Book.Bids[0].price}");
            //    //отправить
            //}

            List<ScanerAddDataRequestDto> result = new();
            foreach(var item in ArbitragePair)
            {
                ScanerAddDataRequestDto line = new()
                {
                    BaseCoinName = item.LongPair.Info.BaseCoin,
                    QuoteCoinName = item.LongPair.Info.QuoteCoin,
                    ExchangeNameLong = item.LongPair.Info.ExchangeName,
                    ExchangeNameShort = item.ShortPair.Info.ExchangeName,
                    MarketTypeLong = item.LongPair.Info.MarketType,
                    MarketTypeShort = item.ShortPair.Info.MarketType,
                    FundingRateLong = 0,
                    FundingRateShort = 0,
                    PurchasePriceLong = item.LongPair.Book.Ask,
                    PurchasePriceShort = item.LongPair.Book.Bid,
                };
                result.Add(line);
            }


            var response = await Network.PostAsync("https://localhost:7102/api/Scaner/scaner", result);
            Console.WriteLine(response.ToString());
        }
    }
}
