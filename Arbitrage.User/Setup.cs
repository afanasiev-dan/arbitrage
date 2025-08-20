using Arbitrage.User.Application.Contracts;
using Arbitrage.User.Application.Services;
using Arbitrage.User.Domain.Contracts;
using Arbitrage.User.Infastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitrage.User
{
    public static class Setup
    {
        public static IServiceCollection AddUserModule(
            this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}