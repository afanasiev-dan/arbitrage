using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Converter;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Dto;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Mappers;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank
{
    public class LBankApiClient(
                IExchangeRepository exchangeRepository,
                ICoinRepository symbolRepository,
                ICurrencyPairRepository currencyPairRepository
                                ) : IExchangeApiClient
    {
        private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
        private readonly ICoinRepository _symbolRepository = symbolRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

        public string Name { get; } = Exchanges.LBank;
        public string BaseUrlSpot => "https://api.lbkex.com/v2/kline.do";
        public string BaseUrlFuture => throw new NotImplementedException();

        public int MaxCandlesSpotPerRequest => 2000;
        public int MaxCandlesFuturePerRequest => throw new NotImplementedException();

        public async Task<List<Candle>> GetSpotCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            // TODO: проверить какую именно дату мы тут передаем и какие данные получаем
            var unixTimeSeconds = new DateTimeOffset(dateFrom).ToUnixTimeSeconds();

            return await GetCandles(unixTimeSeconds, symbolFisrt, symbolSecond, MarketType.Spot);
        }

        public async Task<List<Candle>> GetFutureCandlesAsync(string symbolFisrt, string symbolSecond, DateTime dateFrom, DateTime dateTo)
        {
            throw new NotImplementedException("Для ЛБанка не реализовано получение свечей с фьюючей");
        }

        private async Task<List<Candle>> GetCandles(long unixTimeSeconds, string symbolFirst, string symbolSecond, MarketType marketType)
        {
            List<LBankCandleResponceDto> candlesDto = [];
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
                apiUrl = $"{BaseUrlSpot}?symbol={symbolPair.Pair}&type=minute{Intervals.FiveMinutesValue}&size={MaxCandlesSpotPerRequest}&time={unixTimeSeconds}";
            // else if (marketType == MarketType.Futures)
            //     apiUrl = $"{BaseUrlFuture}/usdt/candlesticks?contract={symbolPair.Pair}&interval={Intervals.FiveMinutesValue}m&from={dateFromSec}&to={dateToSec}";

            try
            {
                var body = await Network.GetAsync(apiUrl);
                candlesDto = LBankCandleDtoConverter.Convert(body);

                foreach (var candleDto in candlesDto)
                {
                    var candle = LBankCandleMapper.ToDomainEntity(candleDto, exchange.Id, symbolPair.Id, Intervals.FiveMinutesValue);
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