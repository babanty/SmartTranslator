using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartTranslator.TelegramBot.Management.GptTelegramBots;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotMessageHandler : IGptTelegramBotMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBotMessageHandler> _logger;


    public TelegramBotMessageHandler(IServiceProvider serviceProvider,
                           ILogger<TelegramBotMessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(new EventId(1000000), exception, errorMessage);
        return Task.CompletedTask;
    }


    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var scope = CreateServiceScope(botClient);
        var incomingMessageHandler = scope.ServiceProvider.GetRequiredService<TelegramIncomingMessageHandler>();

        try
        {
            await incomingMessageHandler.HandleRequest(botClient, update, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(1000001), e, "Processing the telegram message failed. Most likely, the problem is in sending the response");
        }
    }


    public IServiceScope CreateServiceScope(ITelegramBotClient botClient)
    {
        var scope = _serviceProvider.CreateScope();

        scope.ServiceProvider.GetRequiredService<ITelegramBotClientProvider>().Init(botClient);

        return scope;
    }
}

