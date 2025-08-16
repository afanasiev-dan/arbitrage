using Arbitrage.Graph.Domain;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients
{
    public interface IExchangeApiClientFactory
    {
        IExchangeApiClient GetClient(string exchangeName);
    }
}