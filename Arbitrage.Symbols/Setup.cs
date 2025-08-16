using Arbitrage.Symbols.Application.Contracts;
using Arbitrage.Symbols.Application.Services;
using Arbitrage.Symbols.Domain.Contracts;
using Arbitrage.Symbols.Infastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitrage.Symbols;

public static class Setup
{
    public static IServiceCollection AddSymbolsModule(
        this IServiceCollection services)
    {
        services.AddScoped<ICoinService, CoinService>();
        services.AddScoped<ICurrencyPairService, CurrencyPairService>();

        services.AddScoped<ICurrencyPairConverter, CurrencyPairConverter>();
        services.AddScoped<IExchangeFormatRepository, ExchangeFormatRepository>();
        
        services.AddScoped<ICurrencyPairRepository, CurrencyPairRepository>();
        services.AddScoped<ICoinRepository, CoinRepository>();

        return services;
    }
}