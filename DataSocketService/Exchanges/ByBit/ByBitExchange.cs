using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Exchanges.ByBit
{
    internal class ByBitExchange : ExchangeBase
    {
        public ByBitExchange(string exchangeName, MarketType exchangeType, List<CurrencyPairResponceDto> currencyPairs) : base(exchangeName, exchangeType, currencyPairs)
        {
        }

        public override SocketBase CreateSocketBook()
            => new ByBitSocket();
    }
}
