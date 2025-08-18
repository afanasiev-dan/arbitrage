using Arbitrage.Scaner.Application.Contracts;
using Arbitrage.Scaner.Application.Services;
using Arbitrage.Scaner.Domain.Contracts;
using Arbitrage.Scaner.Infastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitrage.Scaner;

public static class Setup
{
    public static IServiceCollection AddScanerModule(
        this IServiceCollection services)
    {
        services.AddTransient<IScanerRepository, ScanerRepository>();
        services.AddScoped<IScanerService, ScanerService>();

        return services;
    }    
}
