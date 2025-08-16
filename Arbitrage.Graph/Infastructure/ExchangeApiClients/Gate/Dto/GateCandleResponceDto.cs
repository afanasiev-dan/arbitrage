using Arbitrage.Graph.Infastructure.ExchangeApiClients;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate.Dto
{
    public class GateCandleResponceDto : CandleResponceDtoBase
    {
        /// <summary>
        /// Объём котируемых активов
        /// </summary>
        public decimal QuoteAssetVolume { get; set; }

        public bool IsClosed { get; set; }
    }
}