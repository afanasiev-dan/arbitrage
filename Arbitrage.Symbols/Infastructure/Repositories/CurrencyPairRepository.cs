using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Symbols.Infastructure.Repositories
{
    public class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly DbContext _context;
        public CurrencyPairRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IEnumerable<CurrencyPair> pair)
        {
            foreach (var p in pair)
                await _context.Set<CurrencyPair>().AddAsync(p);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CurrencyPair>> GetAllAsync()
        {
            return await _context.Set<CurrencyPair>()
                .Include(c => c.BaseCoin)
                .Include(c => c.QuoteCoin)
                .Include(c => c.Exchange)
                .ToArrayAsync();
        }

        public async Task<CurrencyPair?> GetBySymbolAndExchangeAsync(Guid baseSymbolId, Guid quoteSymbolId, Guid exchangeId)
        {
            return await _context.Set<CurrencyPair>()
                .Include(c => c.BaseCoin)
                .Include(c => c.QuoteCoin)
                .Include(c => c.Exchange)
                .Where(c => c.BaseCoinId == baseSymbolId
                            && c.QuoteCoinId == quoteSymbolId
                            && c.ExchangeId == exchangeId)
                .FirstOrDefaultAsync();
        }

        public async Task<CurrencyPair?> GetBySymbolAndExchangeAsync(Guid baseSymbolId, Guid quoteSymbolId, Guid exchangeId, MarketType? marketType)
        {
            var query = _context.Set<CurrencyPair>()
                .Include(c => c.BaseCoin)
                .Include(c => c.QuoteCoin)
                .Include(c => c.Exchange)
                .Where(c => c.BaseCoinId == baseSymbolId
                     && c.QuoteCoinId == quoteSymbolId
                     && c.ExchangeId == exchangeId);

            if (marketType != null)
                query = query.Where(c => c.MarketType == marketType);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<CurrencyPair?> GetByPairAndExchangeAsync(string pair, Guid exchangeId, MarketType? marketType)
        {
            var query = _context.Set<CurrencyPair>()
                .Include(c => c.BaseCoin)
                .Include(c => c.QuoteCoin)
                .Include(c => c.Exchange)
                .Where(c => c.Pair == pair
                     && c.ExchangeId == exchangeId);

            if (marketType != null)
                query = query.Where(c => c.MarketType == marketType);

            return await query.FirstOrDefaultAsync();
        }
        public async Task<CurrencyPair?> GetBySymbolAsync(Guid baseSymbolId, Guid quoteSymbolId)
        {
            return await _context.Set<CurrencyPair>()
                .Include(c => c.BaseCoin)
                .Include(c => c.QuoteCoin)
                .Include(c => c.Exchange)
                .Where(c => c.BaseCoinId == baseSymbolId
                            && c.QuoteCoinId == quoteSymbolId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(IEnumerable<CurrencyPair> pairs)
        {
            _context.Set<CurrencyPair>().UpdateRange(pairs);
            await _context.SaveChangesAsync();
        }
    }
}