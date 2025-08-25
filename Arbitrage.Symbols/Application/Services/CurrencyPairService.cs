using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.Symbols.Application.Contracts;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;

namespace Arbitrage.Symbols.Application.Services
{
    public class CurrencyPairService(
        ICurrencyPairRepository currencyPairRepository,
        IExchangeRepository exchangeRepository,
        ICoinRepository coinRepository
    ) : ICurrencyPairService
    {
        private readonly ICurrencyPairRepository _currencyPairRepository = currencyPairRepository;
        private readonly IExchangeRepository _exchangeRepository = exchangeRepository;
        private readonly ICoinRepository _coinRepository = coinRepository;

        public async Task AddCurrencyPairsAsync(IEnumerable<CurrencyPairRequestDto> currencyPairs)
        {
            IList<CurrencyPair> currencyPairForAdd = [];

            foreach (var pairDto in currencyPairs)
            {
                var coinFirst = await _coinRepository.GetByTickerAsync([pairDto.BaseCoinName]);
                if (coinFirst is null) throw new ArgumentNullException("Монета не найдена");

                var coinSecond = await _coinRepository.GetByTickerAsync([pairDto.QuoteCoinName]);
                if (coinSecond is null) throw new ArgumentNullException("Монета не найдена");

                var exchange = await _exchangeRepository.GetByNameAsync(pairDto.ExchangeName);
                if (exchange is null) throw new ArgumentNullException("Биржа не найдена");

                var symbolPair = await _currencyPairRepository.GetBySymbolAndExchangeAsync(coinFirst.FirstOrDefault()!.Id, coinSecond.FirstOrDefault()!.Id, exchange.Id, pairDto.MarketType);
                if (symbolPair is null) throw new ArgumentNullException("Пара не найдена");

                var currencyPair = new CurrencyPair()
                {
                    Pair = pairDto.Pair,
                    BaseCoinId = coinFirst.FirstOrDefault()!.Id,
                    QuoteCoinId = coinSecond.FirstOrDefault()!.Id,
                    ExchangeId = exchange.Id,
                    MarketType = pairDto.MarketType,
                    exchangeType = pairDto.exchangeType
                };

                currencyPairForAdd.Add(currencyPair);
            }

            await _currencyPairRepository.AddAsync(currencyPairForAdd);
        }

        public async Task<IEnumerable<CurrencyPair>> GetAllCurrencyPairsAsync()
        {
            return await _currencyPairRepository.GetAllAsync();
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(IEnumerable<GetCurrencyPairDto> currencyPairs)
        {
            List<CurrencyPair> result = [];

            foreach (var pair in currencyPairs)
            {
                var coinFirst = await _coinRepository.GetByTickerAsync([pair.BaseCoinName]);
                if (coinFirst is null) throw new ArgumentNullException("Монета не найдена");

                var coinSecond = await _coinRepository.GetByTickerAsync([pair.QuoteCoinName]);
                if (coinSecond is null) throw new ArgumentNullException("Монета не найдена");

                var symbolPair = await _currencyPairRepository.GetBySymbolAsync(coinFirst.FirstOrDefault()!.Id, coinSecond.FirstOrDefault()!.Id);
                if (symbolPair is null) throw new ArgumentNullException("Пара не найдена");

                result.Add(symbolPair);
            }

            return result;
        }

        public async Task UpdateCurrencyPairsAsync(IEnumerable<CurrencyPairRequestDto> currencyPairs)
        {
            IList<CurrencyPair> currencyPairForUpdate = [];

            foreach (var pairDto in currencyPairs)
            {
                var coinFirst = await _coinRepository.GetByTickerAsync([pairDto.BaseCoinName]);
                if (coinFirst is null || !coinFirst.Any())
                    throw new ArgumentNullException("Монета не найдена: " + pairDto.BaseCoinName);

                var coinSecond = await _coinRepository.GetByTickerAsync([pairDto.QuoteCoinName]);
                if (coinSecond is null || !coinSecond.Any())
                    throw new ArgumentNullException("Монета не найдена: " + pairDto.QuoteCoinName);

                var exchange = await _exchangeRepository.GetByNameAsync(pairDto.ExchangeName);
                if (exchange is null)
                    throw new ArgumentNullException("Биржа не найдена: " + pairDto.ExchangeName);

                var symbolPair = await _currencyPairRepository.GetByPairAndExchangeAsync(pairDto.Pair, exchange.Id, pairDto.MarketType);
                if (symbolPair is null)
                    throw new ArgumentNullException("Пара не найдена: " + pairDto.Pair);

                symbolPair.Pair = pairDto.Pair;

                symbolPair.BaseCoinId = coinFirst.FirstOrDefault()!.Id;
                symbolPair.QuoteCoinId = coinSecond.FirstOrDefault()!.Id;

                symbolPair.ExchangeId = exchange.Id;
                symbolPair.MarketType = pairDto.MarketType;
                symbolPair.exchangeType = pairDto.exchangeType;

                currencyPairForUpdate.Add(symbolPair);
            }

            await _currencyPairRepository.UpdateAsync(currencyPairForUpdate);
        }
    }
}