using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Converters;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate.Dto;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate.Mappers;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate;

public class GateApiClient(
        IExchangeRepository exchangeRepository,
        ICoinRepository symbolRepository,
        ICurrencyPairRepository currencyPairRepository) : IExchangeApiClient
{
    private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
    private readonly ICoinRepository _symbolRepository = symbolRepository;
    private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

    public string Name { get; } = Exchanges.Gate;
    public string BaseUrlSpot => "https://api.gateio.ws/api/v4/spot";
    public string BaseUrlFuture => "https://api.gateio.ws/api/v4/futures";

    public int MaxCandlesSpotPerRequest => 1000;
    public int MaxCandlesFuturePerRequest => 2000;

    public async Task<List<Candle>> GetSpotCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
    {
        var dateFromSec = UnixTimeConverter.ToUnixTimeSeconds(dateFrom);
        var dateToSec = UnixTimeConverter.ToUnixTimeSeconds(dateTo);

        return await GetCandles(dateFromSec, dateToSec, symbolFisrt, symbolSecond, MarketType.Spot);
    }

    public async Task<List<Candle>> GetFutureCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
    {
        var dateFromSec = UnixTimeConverter.ToUnixTimeSeconds(dateFrom);
        var dateToSec = UnixTimeConverter.ToUnixTimeSeconds(dateTo);

        return await GetCandles(dateFromSec, dateToSec, symbolFisrt, symbolSecond, MarketType.Futures);
    }

    private async Task<List<Candle>> GetCandles(long dateFromSec, long dateToSec, string symbolFirst, string symbolSecond, MarketType marketType)
    {
        List<GateCandleResponceDto> candlesDto = [];
        List<Candle> candles = [];

        symbolSecond = "USDT";

        var coinFirst = await _symbolRepository.GetByTickerAsync([symbolFirst]);
        if (coinFirst is null) throw new ArgumentNullException("Монета не найдена");

        var coinSecond = await _symbolRepository.GetByTickerAsync([symbolSecond]);
        if (coinSecond is null) throw new ArgumentNullException("Монета не найдена");

        var exchange = await _exchangeRepository.GetByNameAsync(Name);
        if (exchange is null) throw new ArgumentNullException("Биржа не найдена");

        var symbolPair = await _currencyPairRepository.GetBySymbolAndExchangeAsync(coinFirst.FirstOrDefault()!.Id, coinSecond.FirstOrDefault()!.Id, exchange.Id, marketType);
        if (symbolPair is null) throw new ArgumentNullException("Пара не найдена");

        string apiUrl = string.Empty;


        if (marketType == MarketType.Spot)
            apiUrl = $"{BaseUrlSpot}/candlesticks?currency_pair={symbolPair.Pair}&interval={Intervals.FiveMinutesValue}m&from={dateFromSec}&to={dateToSec}";
        else if (marketType == MarketType.Futures)
            apiUrl = $"{BaseUrlFuture}/usdt/candlesticks?contract={symbolPair.Pair}&interval={Intervals.FiveMinutesValue}m&from={dateFromSec}&to={dateToSec}";

        try
        {
            var body = await Network.GetAsync(apiUrl);

            if (marketType == MarketType.Spot)
                candlesDto = GateCandleDtoConverter.SpotConvert(body);
            else if (marketType == MarketType.Futures)
                candlesDto = GateCandleDtoConverter.FutureConvert(body);

            foreach (var candleDto in candlesDto)
            {
                var candle = GateCandleMapper.ToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
                candles.Add(candle);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении свеч для пары: " + ex.Message);
            throw;
        }

        return candles;
    }
}