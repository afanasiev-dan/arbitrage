using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Domain.Contracts
{
    public interface ICoinRepository
    {
        Task<IEnumerable<Coin>> GetAllAsync();
        Task<IEnumerable<Coin?>> GetByTickerAsync(IEnumerable<string> tickers);
        Task AddAsync(IEnumerable<string> symbols);
    }
}