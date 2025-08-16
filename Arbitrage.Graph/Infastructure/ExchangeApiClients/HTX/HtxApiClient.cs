using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Converters;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Dto;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX.Mappers;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX
{
    public class HtxApiClient(
            IExchangeRepository exchangeRepository,
            ICoinRepository symbolRepository,
            ICurrencyPairRepository currencyPairRepository
                             ) : IExchangeApiClient
    {
        private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
        private readonly ICoinRepository _symbolRepository = symbolRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

        public string Name { get; } = Exchanges.Htx;
        public string BaseUrlSpot => "https://api.huobi.pro/market/history/kline";
        public string BaseUrlFuture => "https://api.hbdm.com/linear-swap-ex/market/history/kline";

        public int MaxCandlesSpotPerRequest => 2000;
        public int MaxCandlesFuturePerRequest => 1000;


        public async Task<List<Candle>> GetFutureCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromSec = UnixTimeConverter.ToUnixTimeSeconds(dateFrom);
            var dateToSec = UnixTimeConverter.ToUnixTimeSeconds(dateTo);

            return await GetCandles(dateFromSec, dateToSec, symbolFisrt, symbolSecond, MarketType.Futures);
        }

        public async Task<List<Candle>> GetSpotCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            // У htx нет возможности запросить свечи от какой-то до какой-то даты
            return await GetCandles(-1, -1, symbolFisrt, symbolSecond, MarketType.Spot);
        }

        public async Task<List<Candle>> GetCandles(long dateFromSec, long dateToSec, string symbolFirst, string symbolSecond, MarketType marketType)
        {
            List<HtxCandleResponceDto> candlesDto = [];
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
                apiUrl = $"{BaseUrlSpot}?period={Intervals.FiveMinutesValue}min&size={MaxCandlesSpotPerRequest}&symbol={symbolPair.Pair}";
            else if (marketType == MarketType.Futures)
                apiUrl = $"{BaseUrlFuture}?contract_code={symbolPair.Pair}&period={Intervals.FiveMinutesValue}min&size={MaxCandlesFuturePerRequest}&from={dateFromSec}&to={dateToSec}";


            try
            {
                var body = await Network.GetAsync(apiUrl);
                candlesDto = HtxCandleDtoConverter.Convert(body);

                foreach (var candleDto in candlesDto)
                {
                    var candle = HtxCandleMapper.ToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
                    candles.Add(candle);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении свеч для пар: " + ex.Message);
                throw;
            }

            return candles;
        }
    }
}