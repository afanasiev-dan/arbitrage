using Arbitrage.Graph.Domain;

namespace Arbitrage.Graph.Infastructure.ExchangeApiClients
{
    public class ExchangeApiClientFactory : IExchangeApiClientFactory
    {
        private readonly IEnumerable<IExchangeApiClient> _clients;

        public ExchangeApiClientFactory(IEnumerable<IExchangeApiClient> clients)
        {
            _clients = clients;
        }

        public IExchangeApiClient GetClient(string exchangeName)
        {
            var client = _clients.FirstOrDefault(c =>
                string.Equals(c.Name.ToLower(), exchangeName.ToLower(), StringComparison.OrdinalIgnoreCase));

            if (client == null)
                throw new ArgumentException($"No API client registered for exchange: {exchangeName}");

            return client;
        }
    }
}