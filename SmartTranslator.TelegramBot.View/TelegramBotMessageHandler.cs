using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;
using SmartTranslator.TelegramBot.View.Views;
using SmartTranslator.TranslationCore.Enums;
using SmartTranslator.TranslationCore;
using SmartTranslator.TelegramBot.View.Exceptions;
using Telegram.Bot;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using MediatR;
using OpenAI.ObjectModels.ResponseModels;

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotMessageHandler : IGptTelegramBotMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly GptTranslationOptions _gptTranslationOptions;


    public TelegramBotMessageHandler(IServiceProvider serviceProvider,
                           ILogger<TelegramBotMessageHandler> logger,
                           IOptions<GptTranslationOptions> gptTranslationOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _gptTranslationOptions = gptTranslationOptions?.Value ?? new();
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


    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct, TelegramBotRoutingResolver router, MessageView messageView)
    {
        try
        {
            await HandleRequest(botClient, update, ct, router, messageView);
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(1000001), e, "Обработка сообщения из телеграмм не удалась. Вероятнее всего проблема в отправке ответа");
        }
    }


    private async Task HandleRequest(ITelegramBotClient botClient, Update request, CancellationToken ct, TelegramBotRoutingResolver router, MessageView messageView)
    {
        // TODO [NotImpl] - сделать ограничение на количество запросов от одного пользователя

        using var scope = CreateServiceScope(botClient);

        var chatId = request?.Message?.From?.Id ?? request?.CallbackQuery?.From?.Id;

        var handlingResultMessage = await HandleMessageAndExceptions(request, scope, router);
        var handlingResult = handlingResultMessage.Text;

        await Render(handlingResult, chatId ?? 0, scope, ct, messageView);
    }


    private async Task Render(string handlingResult, long chatId, IServiceScope scope, CancellationToken ct, MessageView messageView)
    {
        if (string.IsNullOrEmpty(handlingResult) || chatId == default)
            return;

        var telegramBotMessageSender = scope.ServiceProvider.GetRequiredService<ITelegramBotMessageSender>();

        await SendMessageWithButtons(chatId, telegramBotMessageSender, ct, messageView);
    }


    private async Task SendMessageWithButtons(long chatId,
                                              ITelegramBotMessageSender telegramBotMessageSender,
                                              CancellationToken ct,
                                              MessageView messageView)
    {
        var text = messageView.Text;
        var markup = messageView.Markup;

        var markupedHandlingResult = $"`{text}`"; // чтобы весь текст мог копироваться и игнор таких проблем, типо не экранированных точек

        await telegramBotMessageSender.Send(markupedHandlingResult, chatId, markup, ct);
    }


    private IServiceScope CreateServiceScope(ITelegramBotClient botClient)
    {
        var scope = _serviceProvider.CreateScope();

        scope.ServiceProvider.GetRequiredService<ITelegramBotClientProvider>().Init(botClient);

        return scope;
    }


    private async Task<MessageView?> HandleMessageAndExceptions(Update update, IServiceScope scope, TelegramBotRoutingResolver router)
    {
        var filtersHandlerChain = scope.ServiceProvider.GetRequiredService<IFiltersHandlerChain>();
        var domainEventDistributor = scope.ServiceProvider.GetRequiredService<IPublisher>();
        var telegramBotViews = scope.ServiceProvider.GetServices<ITelegramBotView>().ToList();
        var messageSender = scope.ServiceProvider.GetRequiredService<ITelegramBotMessageSender>();

        try
        {
            var cancellationTokenSource = await ActivateLoadingAnimation(messageSender, update?.Message?.Chat?.Id);

            var view = await router.RouteMessageOrThrow(update, telegramBotViews);
            var result = view is null ? null : await view.Render(update);

            DeactivateLoadingAnimation(cancellationTokenSource);

            return result;
        }
        catch (Exception e)
        {
            await domainEventDistributor.Publish(new HandleMessageFailedEvent(update, e));

            var message = await filtersHandlerChain.FilterException(e);
            return new MessageView
            {
                Text = message,
                Markup = null
            };
        }
    }

    private async Task<CancellationTokenSource> ActivateLoadingAnimation(ITelegramBotMessageSender messageSender, long? chatId)
    {
        // NOTE: анимация загрузки представляет из себя просто точки, например "....", каждую секунду количество точек становится больше на одну

        CancellationTokenSource cts = new();

        if (chatId is null)
        {
            return cts;
        }

        string loadingMessage = "\\.\\.";
        var loadingMessageId = (await messageSender.SendOrUpdateMessage(loadingMessage, chatId.Value)).MessageId;

        _ = Task.Run(async () =>
        {
            try
            {
                var dotsCounter = 2;
                var maxDots = 15;
                while (!cts.Token.IsCancellationRequested && dotsCounter < maxDots)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);

                    loadingMessage += "\\.";
                    dotsCounter++;

                    await messageSender.SendOrUpdateMessage(loadingMessage, chatId.Value, loadingMessageId);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
            }

            try
            {
                await messageSender.DeleteMessage(chatId.Value, loadingMessageId);
            }
            catch
            {
            }

        }, cts.Token);

        return cts;
    }

    private void DeactivateLoadingAnimation(CancellationTokenSource cancellationTokenSource)
    {
        cancellationTokenSource.Cancel();
    }    
}

