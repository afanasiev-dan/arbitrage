using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.Scaner.Application.Contracts;
using Arbitrage.Scaner.Domain.Contracts;
using Arbitrage.Scaner.Domain.Entities;
using Arbitrage.Scaner.Presentation.Dto;
using Arbitrage.Symbols.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Scaner.Application.Services
{
    public class ScanerService(
        ILogger<ScanerService> logger,
        IScanerRepository scanerRepository,
        ICoinRepository coinRepository,
        IExchangeRepository exchangeRepository,
        ICurrencyPairRepository currencyPairRepository) : IScanerService
    {
        private readonly IScanerRepository _scanerRepository = scanerRepository;
        private readonly ILogger<ScanerService> _logger = logger;
        private readonly ICoinRepository _coinRepository = coinRepository;
        private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;

        public async Task<bool> AddScaners(IEnumerable<ScanerAddDataRequestDto> scanerDto)
        {
            if (scanerDto is null || !scanerDto.Any())
                return false;

            var exchanges = await _exchangeRepository.GetAllAsync();
            var symbols = await _coinRepository.GetAllAsync();
            var currencyPairs = await _currencyPairRepository.GetAllAsync();

            if (exchanges is null || !exchanges.Any())
                throw new ArgumentNullException("Не найдена биржa");

            if (symbols is null || !symbols.Any())
                throw new ArgumentNullException("Не найден символ");

            var scanerModels = new List<ScanerModel>();

            foreach (var scaner in scanerDto)
            {
                var baseCoin = symbols.FirstOrDefault(s => s.Name == scaner.BaseCoinName);
                var quoteCoin = symbols.FirstOrDefault(s => s.Name == scaner.QuoteCoinName);
                var exchangeLong = exchanges.FirstOrDefault(e => e.Name == scaner.ExchangeNameLong);
                var exchangeShort = exchanges.FirstOrDefault(e => e.Name == scaner.ExchangeNameShort);
                var currencyPairLong = currencyPairs.FirstOrDefault(x => x.BaseCoin.Name == baseCoin.Name && x.QuoteCoin.Name == quoteCoin.Name && x.Exchange.Name == exchangeLong.Name);
                var currencyPairShort = currencyPairs.FirstOrDefault(x => x.BaseCoin.Name == baseCoin.Name && x.QuoteCoin.Name == quoteCoin.Name && x.Exchange.Name == exchangeShort.Name);

                if (baseCoin is null || quoteCoin is null || exchangeLong is null || exchangeShort is null || currencyPairLong is null || currencyPairShort is null)
                    throw new ArgumentNullException("Не найден символ или биржa");

                var scanerModel = new ScanerModel()
                {
                    Id = Guid.NewGuid(),
                    BaseCoinId = baseCoin.Id,
                    QuoteCoinId = quoteCoin.Id,
                    ExchangeIdLong = exchangeLong.Id,
                    ExchangeIdShort = exchangeShort.Id,
                    MarketTypeLong = scaner.MarketTypeLong,
                    PurchasePriceLong = scaner.PurchasePriceLong,
                    FundingRateLong = scaner.FundingRateLong,
                    MarketTypeShort = scaner.MarketTypeShort,
                    PurchasePriceShort = scaner.PurchasePriceShort,
                    FundingRateShort = scaner.FundingRateShort,
                    TickerLongId = currencyPairLong.Id,
                    TickerShortId = currencyPairShort.Id,
                };

                scanerModels.Add(scanerModel);
            }

            var result = await _scanerRepository.AddScaners(scanerModels);
            return result;
        }

        public async Task<IEnumerable<ScanerModel>> GetScaners()
        {
            return await _scanerRepository.GetScaners();
        }
    }
}