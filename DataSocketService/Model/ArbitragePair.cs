using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Model
{
    public class ArbitragePair
    {
        public CurrencyPairResponceDto LongPair;
        public CurrencyPairResponceDto ShortPair;
    }
}
