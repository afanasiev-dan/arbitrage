using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Converters;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Converter;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Dto;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Mappers;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc;

public class MexcApiClient(
        IExchangeRepository exchangeRepository,
        ICoinRepository symbolRepository,
        ICurrencyPairRepository currencyPairRepository) : IExchangeApiClient
{
    private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
    private readonly ICoinRepository _symbolRepository = symbolRepository;
    private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

    public string Name { get; } = Exchanges.Mexc;
    public int MaxCandlesPerRequest { get; } = 1000;

    public string BaseUrlSpot => "https://api.mexc.com/api/v3/klines";

    public string BaseUrlFuture => "https://contract.mexc.com/api/v1/contract/kline";

    public int MaxCandlesSpotPerRequest => 1000;

    public int MaxCandlesFuturePerRequest => 2000;

    public async Task<List<Candle>> GetSpotCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
    {
        var dateFromMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateFrom);
        var dateToMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateTo);

        return await GetCandles(dateFromMs, dateToMs, symbolFisrt, symbolSecond, MarketType.Spot);
    }

    public async Task<List<Candle>> GetFutureCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
    {
        var dateFromSec = UnixTimeConverter.ToUnixTimeSeconds(dateFrom);
        var dateToSec = UnixTimeConverter.ToUnixTimeSeconds(dateTo);

        return await GetCandles(dateFromSec, dateToSec, symbolFisrt, symbolSecond, MarketType.Futures);
    }

    private async Task<List<Candle>> GetCandles(long dateFrom, long dateTo, string symbolFirst, string symbolSecond, MarketType marketType)
    {
        List<MexcCandleResponceDto> candlesExchangeDto = [];
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
            apiUrl = $"{BaseUrlSpot}?symbol={symbolPair.Pair}&interval={Intervals.FiveMinutesValue}m&startTime={dateFrom}&endTime={dateTo}";
        else if (marketType == MarketType.Futures)
            apiUrl = $"{BaseUrlFuture}/{symbolPair.Pair}?interval=Min{Intervals.FiveMinutesValue}&start={dateFrom}&end={dateTo}";

        try
        {
            var body = await Network.GetAsync(apiUrl);

            if (marketType == MarketType.Spot)
                candlesExchangeDto = MexcCandleDtoConverter.SpotConvert(body);
            else if (marketType == MarketType.Futures)
                candlesExchangeDto = MexcCandleDtoConverter.FuturesConvert(body);

            foreach (var candleDto in candlesExchangeDto)
            {
                var candle = MexcCandleMapper.ToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
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