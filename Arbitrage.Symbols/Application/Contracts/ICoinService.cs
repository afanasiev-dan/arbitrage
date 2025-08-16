using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Application.Contracts
{
    public interface ICoinService
    {
        Task<IEnumerable<Coin>> GetAllAsync();
        Task<IEnumerable<Coin?>> GetByTickerAsync(IEnumerable<string> tickers);
        Task AddSymbolsAsync(IEnumerable<string> tickers);
    }
}