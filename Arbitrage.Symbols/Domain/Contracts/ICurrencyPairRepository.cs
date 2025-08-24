using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Domain.Contracts
{
    public interface ICurrencyPairRepository
    {
        Task<CurrencyPair?> GetBySymbolAsync(Guid baseSymbolId, Guid quoteSymbolId);
        Task<CurrencyPair?> GetBySymbolAndExchangeAsync(Guid baseSymbolId, Guid quoteSymbolId, Guid exchangeId);
        Task<CurrencyPair?> GetBySymbolAndExchangeAsync(Guid baseSymbolId, Guid quoteSymbolId, Guid exchangeId, MarketType? marketType);
        Task<CurrencyPair?> GetByPairAndExchangeAsync(string pair, Guid exchangeId, MarketType? marketType);
 
        Task AddAsync(IEnumerable<CurrencyPair> pair);
        Task UpdateAsync(IEnumerable<CurrencyPair> pair);
        Task<IEnumerable<CurrencyPair>> GetAllAsync();
    }
}