using Arbitrage.Symbols.Application.Contracts;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Application.Services
{
    public class CurrencyPairConverter : ICurrencyPairConverter
    {
        private readonly IExchangeFormatRepository _exchangeRepo;
        private readonly ICoinRepository _symbolRepository;

        public CurrencyPairConverter(
            IExchangeFormatRepository exchangeFormatRepository,
            ICoinRepository symbolRepository)
        {
            _exchangeRepo = exchangeFormatRepository;
            _symbolRepository = symbolRepository;
        }

        public async Task<string> ToExchangeFormatAsync(CurrencyPair pair, string exchangeName)
        {
            var exchange = await _exchangeRepo.GetByExchangeName(exchangeName);

            var baseTicker = await _symbolRepository.GetByTickerAsync([pair.BaseCoin.Name]);
            var quoteTicker = await _symbolRepository.GetByTickerAsync([pair.QuoteCoin.Name]);

            if (baseTicker == null || quoteTicker == null)
                throw new ArgumentNullException(nameof(baseTicker));

            // TODO: возможно сделать так, чтобы возвращалось множество пар
            return $"{baseTicker.FirstOrDefault().Name}{exchange.CustomSeparator}{quoteTicker.FirstOrDefault().Name}";
        }
    }
}