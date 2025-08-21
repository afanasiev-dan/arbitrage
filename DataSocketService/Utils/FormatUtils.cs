using Arbitrage.Domain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Exchanges.Base;
using DataSocketService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DataSocketService.Other
{
    internal class FormatUtils
    {
        public static decimal ToDec(string value)
        {
            value = value.Replace(" ", "").Replace(",", ".");
            return decimal.Parse(value,
                NumberStyles.Float | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture);
        }

        public static string ExchangeToStr(CurrencyPairResponceDto info)
            => $"{info.ExchangeName}-{info.MarketType}";

        public static string ExchangeToStr(string exchangeName, MarketType marketType)
            => $"{exchangeName}-{marketType}";
    }
}
