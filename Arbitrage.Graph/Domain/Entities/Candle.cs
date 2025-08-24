using Arbitrage.Graph.Domain.Entities;

namespace Arbitrage.Graph.Domain;

public class Candle : CandleBase
{
    /// <summary>
    /// Объём
    /// </summary>
    public decimal Volume { get; set; }
}