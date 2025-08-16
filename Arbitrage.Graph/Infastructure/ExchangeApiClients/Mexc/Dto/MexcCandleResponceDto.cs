namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc.Dto
{
    public class MexcCandleResponceDto : CandleResponceDtoBase
    {
        /// <summary>
        /// Объём котируемых активов
        /// </summary>
       public decimal QuoteAssetVolume { get; set; }

        /// <summary>
        ///  Время закрытия Unix Timestamp
        /// </summary>
        public long CloseTime { get; set; }
    }
}