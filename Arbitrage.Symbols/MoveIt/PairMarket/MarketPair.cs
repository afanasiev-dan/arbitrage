using Arbitrage.ExchangeDomain.Enums;
using Newtonsoft.Json;

namespace Arbitrage.Test.CoinNames
{
    public class MarketPair
    {
        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("quote")]
        public string Quote { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("typeMarket")]
        public MarketType MarketType { get; set; }
    }
}
