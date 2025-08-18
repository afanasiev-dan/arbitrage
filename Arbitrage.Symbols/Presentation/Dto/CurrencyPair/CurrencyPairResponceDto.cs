using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arbitrage.ExchangeDomain.Enums;

namespace Arbitrage.Symbols.Presentation.Dto.CurrencyPair
{
    public class CurrencyPairResponceDto
    {
        /// <summary>
        /// Название биржи лонга
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Название монеты лонга 
        /// </summary>
        public string BaseCoin { get; set; }

        /// <summary>
        /// Название монеты шорта 
        /// </summary>
        public string? QuoteCoin { get; set; } = "USDT";

        public MarketType MarketType { get; set; }

        public string Ticker { get; set; }
    }
}