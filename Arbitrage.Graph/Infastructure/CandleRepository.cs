using System.Data;
using Arbitrage.Graph.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Graph.Infastructure;

public class CandleRepository : ICandleRepository
{
    private readonly DbContext _context;
    private readonly ILogger<CandleRepository> _logger;

    public CandleRepository(
        ILogger<CandleRepository> logger,
        DbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<Candle>> GetCandlesAsync(string exchangeName, string symbolName, DateTime dateFrom, DateTime dateTo)
    {
        return await _context.Set<Candle>()
            .Include(x => x.Exchange)
            .Include(x => x.Pair)
            .Where(c => c.Exchange.Name == exchangeName
                      && c.Pair.Pair == symbolName
                      && c.OpenTime >= dateFrom)
            .OrderBy(c => c.OpenTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Candle> candles)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            await _context.Set<Candle>().AddRangeAsync(candles);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError("Не получилось добавить свечки в базу данных: " + ex);
            throw;
        }
    }

    public async Task<bool> AnyCandlesAsync(string exchangeName, string symbolName, DateTime dateFrom, DateTime dateTo)
    {
        return await _context.Set<Candle>()
            .Include(x => x.Exchange)
            .Include(x => x.Pair)
            .AnyAsync(c => c.Exchange.Name == exchangeName
                        && c.Pair.Pair == symbolName
                        && c.OpenTime >= dateFrom);
    }

    public async Task BulkInsertAsync(IEnumerable<Candle> candles)
    {
        await _context.Set<Candle>().AddRangeAsync(candles);
        await _context.SaveChangesAsync();
    }
    
    public async Task BulkUpdateAsync(IEnumerable<Candle> candles)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var candle in candles)
            {
                await _context.Set<Candle>()
                    .Where(c => c.OpenTime == candle.OpenTime
                                && c.Exchange == candle.Exchange
                                && c.Pair == candle.Pair)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(c => c.Open, candle.Open)
                        .SetProperty(c => c.High, candle.High)
                        .SetProperty(c => c.Low, candle.Low)
                        .SetProperty(c => c.Close, candle.Close)
                        .SetProperty(c => c.Volume, candle.Volume));
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError("Не получилось обновить свечки в базe данных: " + ex);
        }
    }
}