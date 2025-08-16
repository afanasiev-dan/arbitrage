using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Application.Contracts
{
    public interface ICurrencyPairConverter
    {
        Task<string> ToExchangeFormatAsync(CurrencyPair pair, string exchangeName);
    }
}