using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Symbols.Infastructure.Repositories
{
    public class ExchangeFormatRepository : IExchangeFormatRepository
    {
        private readonly DbContext _context;
        public ExchangeFormatRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExchangeFormat exchangeFormat)
        {
            await _context.Set<ExchangeFormat>().AddAsync(exchangeFormat);
            await _context.SaveChangesAsync();
        }

        public async Task<ExchangeFormat?> GetByExchangeName(string exchangeName)
        {
            return await _context.Set<ExchangeFormat>()
                .Where(en => en.Name.ToString() == exchangeName.ToString())
                .FirstOrDefaultAsync();
        }
    }
}