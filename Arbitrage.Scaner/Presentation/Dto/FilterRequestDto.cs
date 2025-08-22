using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Scaner.Presentation.Dto
{
    public class FilterRequestDto
    {
        public List<string>? SpotExchanges { get; set; }
        public List<string>? FuturesExchanges { get; set; }
        public decimal? MinArbitrageRate { get; set; }
    }
}
