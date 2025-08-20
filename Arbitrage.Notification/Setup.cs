using Arbitrage.Notification.Domain.Contracts;
using Arbitrage.Notification.Infastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitrage.Notification
{
    public static class Setup
    {
        public static IServiceCollection AddNotificationModule(
            this IServiceCollection services)
        {
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}