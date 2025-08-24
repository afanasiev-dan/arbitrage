using Arbitrage.Notification.Domain.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Arbitrage.Notification.Infastructure.TelegramBot
{
    // public class TelegramBackgroundService(
    //     ITelegramBotClient botClient,
    //     ITelegramBotHandler handler,
    //     ILogger<TelegramBackgroundService> logger
    // ) : BackgroundService
    // {
    //     private readonly ITelegramBotClient _botClient = botClient;
    //     private readonly ITelegramBotHandler _handler = handler;
    //     private readonly ILogger<TelegramBackgroundService> _logger = logger;
    //     private CancellationTokenSource _cts;


    //     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //     {
    //         _logger.LogInformation("Телеграм бот запущен");
    //         _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            
    //         _botClient.StartReceiving(
    //             _handler.HandleUpdateAsync,
    //             _handler.HandleErrorAsync);

    //         try
    //         {
    //             var tcs = new TaskCompletionSource<bool>();
    //             stoppingToken.Register(s => tcs.SetResult(true), tcs);
    //             await tcs.Task;
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError($"Ошибка при работе бота: {ex.Message}", ex);
    //             throw;
    //         }
    //         finally
    //         {
    //             _logger.LogInformation("Телеграм бот остановлен");
    //             _cts.Cancel(); 
    //         }
    //     }
    // }

    // public class TelegramHostedService(
    //     ITelegramBotClient botClient,
    //     ITelegramBotHandler handler,
    //     ILogger<TelegramHostedService> logger
    // ) : IHostedService, IDisposable
    // {
    // private readonly ITelegramBotClient _botClient = botClient;
    // private readonly ITelegramBotHandler _handler = handler;
    // private readonly ILogger<TelegramHostedService> _logger = logger;
    // private CancellationTokenSource _cts;

    // // public TelegramHostedService(
    // //     ITelegramBotClient botClient,
    // //     ITelegramBotHandler handler,
    // //     ILogger<TelegramHostedService> logger)
    // // {
    // //     _botClient = botClient;
    // //     _handler = handler;
    // //     _logger = logger;
    // // }

    // public Task StartAsync(CancellationToken cancellationToken)
    // {
    //     _logger.LogInformation("Телеграм бот запущен");
    //     _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    //     // Метод StartReceiving запускает фоновую задачу и возвращает управление
    //     _botClient.StartReceiving(
    //         updateHandler: _handler.HandleUpdateAsync,
    //         errorHandler: _handler.HandleErrorAsync,
    //         receiverOptions: null,
    //         cancellationToken: _cts.Token
    //     );

    //     return Task.CompletedTask;
    // }

    // public Task StopAsync(CancellationToken cancellationToken)
    // {
    //     _logger.LogInformation("Bot is stopping");
    //     _cts?.Cancel();
    //     return Task.CompletedTask;
    // }

    // public void Dispose()
    // {
    //     _cts?.Dispose();
    // }
    // }
}