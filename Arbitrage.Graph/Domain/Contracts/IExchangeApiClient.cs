using Arbitrage.Graph.Infastructure.ExchangeApiClients;

namespace Arbitrage.Graph.Domain;

public interface IExchangeApiClient
{
    string BaseUrlSpot { get; }
    string BaseUrlFuture { get; }
    string Name { get; }
    int MaxCandlesSpotPerRequest { get; }
    int MaxCandlesFuturePerRequest { get; }

    Task<List<Candle>> GetSpotCandlesAsync(
        string symbolFisrt,
        string symbolSecond,
        DateTime dateFrom,
        DateTime dateTo);

    Task<List<Candle>> GetFutureCandlesAsync(
        string symbolFisrt,
        string symbolSecond,
        DateTime dateFrom,
        DateTime dateTo);
}