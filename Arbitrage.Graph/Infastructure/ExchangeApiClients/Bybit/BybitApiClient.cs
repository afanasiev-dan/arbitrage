using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Converters;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Converter;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Dto;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit.Mappers;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit
{
    public class BybitApiClient(
        IExchangeRepository exchangeRepository,
        ICoinRepository symbolRepository,
        ICurrencyPairRepository currencyPairRepository
                                ) : IExchangeApiClient
    {
        private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
        private readonly ICoinRepository _symbolRepository = symbolRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

        public string Name { get; } = Exchanges.ByBit;
        public string BaseUrlSpot => "https://api.bybit.com/v5/market/kline";
        public string BaseUrlFuture => BaseUrlSpot;

        public int MaxCandlesSpotPerRequest => 1000;
        public int MaxCandlesFuturePerRequest => MaxCandlesSpotPerRequest;

        public async Task<List<Candle>> GetSpotCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateFrom);
            var dateToMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateTo);

            return await GetCandles(dateFromMs, dateToMs, symbolFisrt, symbolSecond, MarketType.Spot);
        }

        public async Task<List<Candle>> GetFutureCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateFrom);
            var dateToMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateTo);

            return await GetCandles(dateFromMs, dateToMs, symbolFisrt, symbolSecond, MarketType.Futures);
        }

        private async Task<List<Candle>> GetCandles(long dateFromMs, long dateToMs, string symbolFirst, string symbolSecond, MarketType marketType)
        {
            List<BybitCandleResponceDto> candlesDto = [];
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
                apiUrl = $"{BaseUrlFuture}?category=spot&symbol={symbolPair.Pair}&interval={Intervals.FiveMinutesValue}&start={dateFromMs}&end={dateToMs}";
            else if (marketType == MarketType.Futures)
                apiUrl = $"{BaseUrlFuture}?category=linear&symbol={symbolPair.Pair}&interval={Intervals.FiveMinutesValue}&start={dateFromMs}&end={dateToMs}";

            try
                {
                    var body = await Network.GetAsync(apiUrl);
                    candlesDto = BybitCandleDtoConverter.Convert(body);

                    foreach (var candleDto in candlesDto)
                    {
                        var candle = BybitCandleMapper.ToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
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
}