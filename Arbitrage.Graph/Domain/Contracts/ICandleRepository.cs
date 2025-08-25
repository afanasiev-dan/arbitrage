using Arbitrage.Graph.Domain.Entities;

namespace Arbitrage.Graph.Domain;

public interface ICandleRepository
{
    Task<List<Candle>> GetCandlesAsync(string exchange, string symbol, DateTime dateFrom, DateTime dateTo);
    Task AddRangeAsync(IEnumerable<Candle> candles);
    Task<bool> AnyCandlesAsync(string exchange, string symbol, DateTime dateFrom, DateTime dateTo);
    Task BulkInsertAsync(IEnumerable<Candle> candles);
    Task BulkUpdateAsync(IEnumerable<Candle> candles);
    Task DeleteRangeAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<ArbitrageCandle>> AddRangeAsync(IEnumerable<ArbitrageCandle> candles);
    Task DeleteArbitrageCandlesRangeAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<ArbitrageCandle>> GetAllAsync();
    Task UpdateRangeAsync(IEnumerable<ArbitrageCandle> candles);
}