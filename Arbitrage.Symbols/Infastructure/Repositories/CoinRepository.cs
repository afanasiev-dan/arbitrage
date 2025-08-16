using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Symbols.Infastructure.Repositories
{
    public class CoinRepository : ICoinRepository
    {
        private readonly DbContext _context;
        public CoinRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Coin symbol)
        {
            await _context.Set<Coin>().AddAsync(symbol);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Coin>> GetAllAsync()
        {
            return await _context.Set<Coin>().ToArrayAsync();
        }

        public async Task<IEnumerable<Coin?>> GetByTickerAsync(IEnumerable<string> tickers)
        {
            return await _context.Set<Coin>()
                .Where(en => tickers
                    .Select(t => t.ToLower())
                    .Contains(en.Name.ToLower()))
                .ToArrayAsync();
        }

        public async Task AddAsync(IEnumerable<string> symbols)
        {
            IEnumerable<Coin> symbolsDb = await GetAllAsync();

            var symbolsDbNames = symbolsDb.Select(s => s.Name);

            var symbolsNameToAdd = symbols.Where(s => !symbolsDbNames.Contains(s));

            foreach (var symbol in symbolsNameToAdd)
                await _context.Set<Coin>().AddAsync(new Coin
                {
                    Id = Guid.NewGuid(),
                    Name = symbol,
                });

            await _context.SaveChangesAsync();
        }
    }
}