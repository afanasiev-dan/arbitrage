namespace Arbitrage.Graph.Domain;

public interface ICandleRepository
{
    Task<List<Candle>> GetCandlesAsync(string exchange, string symbol, DateTime dateFrom, DateTime dateTo);
    Task AddRangeAsync(IEnumerable<Candle> candles);
    Task<bool> AnyCandlesAsync(string exchange, string symbol, DateTime dateFrom, DateTime dateTo);
    Task BulkInsertAsync(IEnumerable<Candle> candles);
    Task BulkUpdateAsync(IEnumerable<Candle> candles);
}