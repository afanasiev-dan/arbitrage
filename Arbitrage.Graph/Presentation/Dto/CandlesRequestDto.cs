using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Presentation.Validations;

namespace Arbitrage.Graph.Presentation.Dto
{
    public class CandlesRequestDto
    {
        public string ExchangeName { get; set; }
        public string SymbolName { get; set; }

        [UtcDateTime]
        public DateTime? DateFrom { get; set; }

        [UtcDateTime]
        public DateTime? DateTo { get; set; }
        public MarketType MarketType { get; set; }
    }
}