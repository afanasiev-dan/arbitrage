using Arbitrage.Notification.Application.Contracts;
using Arbitrage.Notification.Domain.Contracts;
using Arbitrage.Notification.Infastructure.Repositories;
using Arbitrage.Notification.Infastructure.TelegramBot;
using Arbitrage.Notification.Infastructure.TelegramBot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Arbitrage.Notification
{
    public static class Setup
    {
        public static IServiceCollection AddNotificationModule(
            this IServiceCollection services)
        {
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITelegramUserSettingsService, TelegramUserSettingsService>();
            services.AddTransient<ITelegramUserSettingsRepository, TelegramUserSettingsRepository>();

            AddTelegramCommands(services);
            services.AddTransient<ITelegramBotHandler, TelegramBotHandler>();
            services.AddSingleton<ITelegramBotClient>(provider =>
                new TelegramBotClient("7794103266:AAGy3miKD37U1hsYWxNR_gimxr1_t7d_Mds"));
            // services.AddHostedService<TelegramBackgroundService>();
            // services.AddHostedService<TelegramHostedService>();

            return services;
        }

        private static void AddTelegramCommands(IServiceCollection services)
        {
            services.AddTransient<ITelegramCommand ,StartBotCommand>();
            services.AddTransient<ITelegramCommand, SetUsernameCommand>();
            services.AddTransient<CommandsManager>();
        }
    }
}