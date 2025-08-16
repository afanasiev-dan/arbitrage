using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Converters;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Dto;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Mappers;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Converter
{
    public class KuCoinApiClient(
            IExchangeRepository exchangeRepository,
            ICoinRepository symbolRepository,
            ICurrencyPairRepository currencyPairRepository
                ) : IExchangeApiClient
    {
        private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
        private readonly ICoinRepository _symbolRepository = symbolRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

        public string BaseUrlSpot { get; } = "https://api.kucoin.com";
        public string BaseUrlFuture { get; } = "https://api-futures.kucoin.com";
        public string Name { get; } = Exchanges.KuCoin;

        public int MaxCandlesSpotPerRequest => 1500;
        public int MaxCandlesFuturePerRequest => 500;

        public async Task<List<Candle>> GetSpotCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromSec= UnixTimeConverter.ToUnixTimeSeconds(dateFrom);
            var dateToSec= UnixTimeConverter.ToUnixTimeSeconds(dateTo);

            return await GetCandles(dateFromSec, dateToSec, symbolFisrt, symbolSecond, MarketType.Spot);

        }

        // Получше проверить запрос
        public async Task<List<Candle>> GetFutureCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateFrom);
            var dateToMs = UnixTimeConverter.ToUnixTimeMilliseconds(dateTo);

            return await GetCandles(dateFromMs, dateToMs, symbolFisrt, symbolSecond, MarketType.Futures);
        }

        private async Task<List<Candle>> GetCandles(long dateFrom, long dateTo, string symbolFirst, string symbolSecond, MarketType marketType)
        {
            List<KuCoinCandleResponceDto> candlesDto = [];
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
                apiUrl = $"{BaseUrlSpot}/api/v1/market/candles?type={Intervals.FiveMinutesValue}min&symbol={symbolPair.Pair}&startAt={dateFrom}&endAt={dateTo}";
            else if (marketType == MarketType.Futures)
                apiUrl = $"{BaseUrlFuture}/api/v1/kline/query?symbol={symbolPair.Pair}&granularity={Intervals.FiveMinutesValue}&from={dateFrom}&to={dateTo}";

            try
            {
                var body = await Network.GetAsync(apiUrl);
                
                if(marketType == MarketType.Spot)
                    candlesDto = KuCoinCandleDtoConverter.SpotConvert(body);
                else if(marketType == MarketType.Futures)
                    candlesDto = KuCoinCandleDtoConverter.FutureConvert(body);

                foreach (var candleDto in candlesDto)
                {
                    Candle? candle = new(); 
                    if (marketType == MarketType.Spot)
                        candle = KuCoinCandleMapper.SpotToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
                    else if (marketType == MarketType.Futures)
                        candle = KuCoinCandleMapper.FutureToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
                    
                    if (candle is not null)
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