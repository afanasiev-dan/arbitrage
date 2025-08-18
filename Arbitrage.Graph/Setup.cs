using Arbitrage.Graph.Application;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure;
using Arbitrage.Graph.Infastructure.ExchangeApiClients;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Bybit;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Gate;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.HTX;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.KuCoin.Converter;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank;
using Arbitrage.Graph.Infastructure.ExchangeApiClients.Mexc;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitrage.Graph;

public static class Setup
{
    public static IServiceCollection AddGraphModule(
        this IServiceCollection services)
    {
        services.AddTransient<ICandleRepository, CandleRepository>();
        services.AddScoped<CandleService>();

        services.AddScoped<IExchangeApiClient, MexcApiClient>();
        services.AddScoped<IExchangeApiClient, GateApiClient>();
        services.AddScoped<IExchangeApiClient, LBankApiClient>();
        services.AddScoped<IExchangeApiClient, BybitApiClient>();
        services.AddScoped<IExchangeApiClient, KuCoinApiClient>();
        services.AddScoped<IExchangeApiClient, HtxApiClient>();

        services.AddScoped<IExchangeApiClientFactory, ExchangeApiClientFactory>();
        return services;
    }    
}
