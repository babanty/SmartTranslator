using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.Extensions.Options;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <inheritdoc/>
public class GptTelegramBotBuilder : IGptTelegramBotBuilder
{
    private IGptTelegramBotMessageHandler _gptTelegramBotMessageHandler;
    private readonly GptTelegramBotOptions _botOptions;

    public GptTelegramBotBuilder(IGptTelegramBotMessageHandler gptTelegramBotMessageHandler,
                                 IOptions<GptTelegramBotOptions> botOptions)
    {
        _gptTelegramBotMessageHandler = gptTelegramBotMessageHandler;
        _botOptions = botOptions.Value;
    }

    /// <inheritdoc/>
    public async Task<(TelegramBotClient, CancellationTokenSource)> Build()
    {
        var botClient = new TelegramBotClient(_botOptions.AccessToken);

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        using CancellationTokenSource cts = new();

        botClient.StartReceiving(
            updateHandler: _gptTelegramBotMessageHandler.HandleUpdateAsync,
            pollingErrorHandler: _gptTelegramBotMessageHandler.HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");

        return (botClient, cts);
    }
}
