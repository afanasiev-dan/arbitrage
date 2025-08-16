using System.Dynamic;
using Arbitrage.Symbols.Application.Contracts;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Application.Services
{
    public class CoinService : ICoinService
    {
        private readonly ICoinRepository _symbolRepository;

        public CoinService(ICoinRepository symbolRepository)
        {
            _symbolRepository = symbolRepository;
        }

        public async Task AddSymbolsAsync(IEnumerable<string> tickers)
        {
            await _symbolRepository.AddAsync(tickers);
        }

        public async Task<IEnumerable<Coin>> GetAllAsync()
        {
            return await _symbolRepository.GetAllAsync(); 
        }

        public async Task<IEnumerable<Coin?>> GetByTickerAsync(IEnumerable<string> tickers)
        {
            return await _symbolRepository.GetByTickerAsync(tickers) ?? [];
        }
    }
}