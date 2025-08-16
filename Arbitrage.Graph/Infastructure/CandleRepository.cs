using Arbitrage.Graph.Domain;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.Graph.Infastructure;

public class CandleRepository : ICandleRepository
{
    private readonly DbContext _context;

    public CandleRepository(DbContext context)
    {
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
        await _context.Set<Candle>().AddRangeAsync(candles);
        await _context.SaveChangesAsync();
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
        // Для EF Core 7.0+ можно использовать ExecuteUpdateAsync
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
    }
}