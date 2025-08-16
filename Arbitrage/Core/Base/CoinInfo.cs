using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Service.Base
{
    public class CoinInfo
    {
        public string Ticker { get; set; }
        public string BaseCoin { get; set; }
        public string QuoteCoin { get; set; }
        public decimal PriceTick { get; set; }
        public decimal Volume24H { get; set; }
        public decimal Multiplier { get; set; }

        public CoinInfo()
        {
            Multiplier = 1;
        }
        //public List<string> Adresses { get; set; } = new();
        //public List<string> SourceExchanges { get; set; } = new();
    }
}