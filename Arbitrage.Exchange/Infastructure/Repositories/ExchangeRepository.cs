using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Exchange.Infastructure.Repositories
{
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly DbContext _context;
        public ExchangeRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IEnumerable<ExchangeModel> exchanges)
        {
            await _context.Set<ExchangeModel>().AddRangeAsync(exchanges);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ExchangeModel>> GetAllAsync()
        {
            return await _context.Set<ExchangeModel>().ToListAsync();
        }

        public async Task<ExchangeModel?> GetByNameAsync(string name)
        {
            return await _context.Set<ExchangeModel>()
                .Where(e => e.Name == name)
                .FirstOrDefaultAsync();
        }
    }
}