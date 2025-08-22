using Arbitrage.Scaner.Domain.Contracts;
using Arbitrage.Scaner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Scaner.Infastructure.Repositories
{
    public class ScanerRepository : IScanerRepository
    {
        private readonly DbContext _context;
        private readonly ILogger<ScanerRepository> _logger;

        public ScanerRepository(
            ILogger<ScanerRepository> logger,
            DbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> AddScaners(IEnumerable<ScanerModel> scanerModels)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM ScanerData");

                await _context.Set<ScanerModel>().AddRangeAsync(scanerModels);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Не получилось обновить данные сканера в базе данных:" + ex);
                return false;
            }


            // try
            // {
            //     await _context.Set<ScanerModel>().AddRangeAsync(scanerModels);
            //     await _context.SaveChangesAsync();

            //     return true;
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError("Не получилось добавить данные сканера в базу данных: " + ex);
            //     return false;
            // }
        }

        public async Task<IEnumerable<ScanerModel>> GetScaners()
        {
            return await _context.Set<ScanerModel>()
              .Include(x => x.BaseCoin)
              .Include(x => x.QuoteCoin)
              .Include(x => x.ExchangeLong)
              .Include(x => x.ExchangeShort)
              .Include(x => x.TickerLong)
              .Include(x => x.TickerShort)
              .AsNoTracking()
              .ToListAsync();
        }
    }
}
