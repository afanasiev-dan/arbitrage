using Arbitrage.ExchangeDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Model
{
    public class LineResult
    {
        public string BaseCoin;
        public string QuoteCoin;

        public string ExchangeNameLong;
        public MarketType ExchangeTypeLong;
        public string ExchangeNameShort;
        public MarketType ExchangeTypeShort;

        public decimal Ask;
        public decimal Bid;

        public decimal FundingLong;
        public decimal FundingShort;
    }
}
