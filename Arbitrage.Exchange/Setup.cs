using Arbitrage.Exchange.Application.Contracts;
using Arbitrage.Exchange.Application.Services;
using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.Exchange.Infastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitrage.Exchange;

public static class Setup
{
    public static IServiceCollection AddExchangeModule(
        this IServiceCollection services)
    {
        services.AddScoped<IExchangeRepository, ExchangeRepository>();
        services.AddScoped<IExchangeService, ExchangeService>();
        return services;
    }    
}
