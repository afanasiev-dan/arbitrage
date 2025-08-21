using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Other;

namespace DataSocketService.Exchanges.Base
{
    public static class ExchangeFactory
    {
        public static ExchangeBase Create(string exchangeName, MarketType type, List<CurrencyPairResponceDto> pairs)
        {
            string name = FormatUtils.ExchangeToStr(exchangeName,type);
            var socketCreator = LaunchConfig.SocketSettings[name].SocketCreator;
            return new GenericExchange(exchangeName, type, pairs, socketCreator);
        }
    }

    public class GenericExchange : ExchangeBase
    {
        public GenericExchange(string name, MarketType type, List<CurrencyPairResponceDto> pairs, Func<SocketBase> socketCreator)
            : base(name, type, pairs, socketCreator)
        {
        }

        public override SocketBase CreateSocketBook() => CreateSocket();
    }

}
