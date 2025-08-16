using Arbitrage.Core.Base.Enums;
using Arbitrage.Core.Calclucation;
using Arbitrage.Other.Telegram;
using Arbitrage.Service.Base;
using ArbitrageSignalBot;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


public class TelegramAPI
{
    private const string tokenBot = "8015346245:AAG7rpdkLf9un9-zAhQYDhXq_KrL3_cbnmo";
    TelegramBotClient botClient;
    public Func<long, string, Task> OnMessageReceived;
    public Func<long, string, Task> OnSignalRemoved;
    Action<long, Signal> OnSignalClick = Program.PrintSignalByBtn;

    public TelegramAPI()
    {
        botClient = new TelegramBotClient(tokenBot);
        var receiverOptions = new ReceiverOptions
        {
            DropPendingUpdates = true,// Пропустить все старые сообщения при старте
        };

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cToken)
    {
        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            var userId = update.Message.Chat.Id;
            var userMessage = update.Message.Text;
            OnMessageReceived?.Invoke(userId, userMessage);
        }

        //"REMOVE 367717889 ADA Mexc Spot Mexc Futures - 12 0"
        //"PRINT 367717889 MAX Gateio Spot Gateio Futures  0 11,0"
        if (update.Type == UpdateType.CallbackQuery)
        {
            var userId = update.CallbackQuery.From.Id;
            var userMessage = update.CallbackQuery.Data.Split(' ');
            var messageId = update.CallbackQuery.Message.MessageId;

            TypeBtn type = TypeBtn.Print;
            switch (userMessage[0])
            {
                case "RS":
                    type = TypeBtn.RemoveSignal;
                    break;
                case "R":
                    type = TypeBtn.Refresh;
                    break;
                case "P":
                    type = TypeBtn.Print;
                    break;
                case "D":
                    type = TypeBtn.DeleteMessage;
                    break;
                case "M":
                    type = TypeBtn.Mute;
                    break;
            }

            switch (type)
            {
                case TypeBtn.RemoveSignal:
                    var id = string.Join(" ", userMessage.Skip(1));
                    OnSignalRemoved?.Invoke(userId, id);
                    break;
                case TypeBtn.Print:
                    PreSignal preSignal = new PreSignal();
                    preSignal.Name = userMessage[1];
                    preSignal.ExchangeLong = (ExchangeEnum)Enum.Parse(typeof(ExchangeEnum), userMessage[2]);
                    preSignal.AssetTypeLong = (AssetTypeEnum)Enum.Parse(typeof(AssetTypeEnum), userMessage[3]);
                    preSignal.ExchangeShort = (ExchangeEnum)Enum.Parse(typeof(ExchangeEnum), userMessage[4]);
                    preSignal.AssetTypeShort = (AssetTypeEnum)Enum.Parse(typeof(AssetTypeEnum), userMessage[5]);
                    preSignal.Comment = userMessage[6];
                    preSignal.Target = decimal.Parse(userMessage[7]);

                    Signal signal = new(preSignal);
                    signal.TargetVolume = decimal.Parse(userMessage[8]);
                    OnSignalClick?.Invoke(userId, signal);
                    break;
                case TypeBtn.Refresh:
                    //Console.WriteLine($"{userId} || {chatId} ");
                    var baseCoin = userMessage[1];
                    await Program.SignalProcessor.ActivateSignal(userId, messageId, baseCoin);
                    break;
                case TypeBtn.DeleteMessage:
                    await botClient.DeleteMessage(chatId: userId, messageId: messageId);
                    break;
                case TypeBtn.Mute:
                    var id2 = string.Join(" ", userMessage.Skip(1));
                    var new_time = Program.SignalProcessor.telegramBot.Mute(id2);

                    string ticker = id2.Split(" ")[0];
                    SendMessage(userId, $"{ticker} заглушен на 2 часа до {new_time:HH:mm}");
                    break;
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiEx => $"Telegram API Error: {apiEx.ErrorCode} - {apiEx.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
    public async Task SendMessage(long chatId, string message)
    {
        await botClient.SendMessage(
           chatId: chatId,
           text: message);
    }
    public async Task SendMessageAsync(long chatId, string message, State state = State.MainMenu, Dictionary<string, string> arguments = null, Settings settings = null, StateSignal stateSignal = StateSignal.None, int? messageId = null)
    {
        bool printMessage = stateSignal != StateSignal.None;
        if (printMessage)
            state = State.PrintSignals;
        if (state == State.EnteringResult || state == State.EnteringComment)
            printMessage = true;

        try
        {
            // 1. Подготовка Reply-клавиатуры (если нужна)
            ReplyKeyboardMarkup? replyKeyboardMarkup = null;
            bool hideKeyboard = false;

            switch (state)
            {
                case State.EnteringShortExchange:
                case State.EnteringLongExchange:
                    replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                    new KeyboardButton[] { "KuCoin", "Mexc", "Gateio", "ByBit", "HTX", "LBank" },
                    new KeyboardButton[] { "KuCoin#F", "Mexc#F", "Gateio#F", "ByBit#F", "HTX#F", "LBank#F" },
                    })
                    {
                        ResizeKeyboard = true
                    };
                    break;
                case State.EnteringComment:
                case State.AddingSignal:
                case State.EnteringResult:
                    hideKeyboard = true;
                    break;
                case State.InputVolume:
                case State.InputInterval:
                case State.InputProfit:
                case State.Settings:
                    if (settings == null)
                        return;
                    replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                    new KeyboardButton[] { $"Интервал между сигналами ({settings.interval})",  $"% прибыли ({settings.profit})", $"объем ({settings.volume})" }, // $"% проскальзывания ({settings.slippage})", 
                    new KeyboardButton[] { $"Работает: {(settings.isWork?"✅": "❌")}", "Назад"},
                    })
                    {
                        ResizeKeyboard = true
                    };
                    break;
                case State.EnteringDirectSignal:
                    replyKeyboardMarkup = new ReplyKeyboardMarkup([["Вход", "Выход"]])
                    {
                        ResizeKeyboard = true
                    };
                    break;
                default:
                    if (stateSignal == StateSignal.None)
                    {
                        replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                        {
                        new KeyboardButton[] { "Добавить", "Список" },
                        new KeyboardButton[] { "Настройки" }
                        })
                        {
                            ResizeKeyboard = true
                        };
                    }
                    break;
            }
            string TruncateMessage(string msg, int maxLength = 4096)
            {
                return msg.Length > maxLength ? msg.Substring(0, maxLength) : msg;
            }

            if (stateSignal != StateSignal.None)
            {
                printMessage = false;
                message = EscapeMarkdown(message);
                message = TruncateMessage(message);

                var buttons = new List<InlineKeyboardButton[]>();
                if (arguments != null && arguments.TryGetValue("🔄", out var refreshValue) && arguments.TryGetValue("❌", out var cancelValue)
                    && arguments.TryGetValue("🔕", out var muteValue))
                {
                    buttons.Add(
                    [
                        InlineKeyboardButton.WithCallbackData("🔄", refreshValue),
                        InlineKeyboardButton.WithCallbackData("❌", cancelValue),
                        InlineKeyboardButton.WithCallbackData("🔕", muteValue),
                    ]);
                    arguments.Remove("🔄");
                    arguments.Remove("❌");
                    arguments.Remove("🔕");
                    foreach (var arg in arguments)
                        buttons.Add([InlineKeyboardButton.WithCallbackData(arg.Key, arg.Value)]);
                }

                var inlineKeyboard = new InlineKeyboardMarkup(buttons);
                if (messageId == null)
                {
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: message,
                        replyMarkup: inlineKeyboard,
                        linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                        parseMode: ParseMode.MarkdownV2);
                }
                else
                {
                    await botClient.EditMessageText(
                        chatId: chatId,
                        messageId: (int)messageId,
                        text: message,
                        replyMarkup: inlineKeyboard,
                        linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                        parseMode: ParseMode.MarkdownV2);
                }
            }

            if (replyKeyboardMarkup != null && !hideKeyboard)
            {
                message = TruncateMessage(message);
                await botClient.SendMessage(
                    chatId: chatId,
                         text: message,
                    replyMarkup: replyKeyboardMarkup);
            }
            else if (printMessage)
            {
                message = TruncateMessage(message);
                await botClient.SendMessage(
                           chatId: chatId,
                           text: message,
                           replyMarkup: new ReplyKeyboardRemove());
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
        }
    }

    string EscapeMarkdown(string input)
    {
        var reservedChars = new[] { "_", "*", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", "!" };
        foreach (var c in reservedChars)
        {
            input = input.Replace(c, "\\" + c);
        }
        return input;
    }

    enum TypeBtn
    {
        RemoveSignal, Print, Refresh, DeleteMessage, Mute
    }
}