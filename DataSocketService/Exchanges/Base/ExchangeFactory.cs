using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.ByBit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Exchanges.Base
{
    public static class ExchangeFactory
    {
        public static ExchangeBase Create(string exchangeName, MarketType exchangeType, List<CurrencyPairResponceDto> currencyPairs)
        {
            switch (exchangeName)
            {
                case Arbitrage.ExchangeDomain.Exchanges.ByBit:
                    return new ByBitExchange(exchangeName, exchangeType, currencyPairs);
                //case Exchanges.Gate:
                //    return new GateExchange(exchangeName, exchangeType, currencyPairs);
                //case Exchanges.Mexc:
                //    return new MexcExchange(exchangeName, exchangeType, currencyPairs);
                //case Exchanges.KuCoin:
                //    return new KuCoinExchange(exchangeName, exchangeType, currencyPairs);
                //case Exchanges.LBank:
                //    return new LBankExchange(exchangeName, exchangeType, currencyPairs);
                //case Exchanges.Htx:
                //    return new HtxExchange(exchangeName, exchangeType, currencyPairs);
                //case Exchanges.Binance:
                //    return new BinanceExchange(exchangeName, exchangeType, currencyPairs);

                default:
                    throw new ArgumentException($"Неизвестная биржа: {exchangeName}");
            }
        }
    }

}
