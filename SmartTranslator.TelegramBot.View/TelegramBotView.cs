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

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotView : BackgroundService, IGptTelegramBotMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBotView> _logger;
    private readonly GptTranslationOptions _gptTranslationOptions;


    public TelegramBotView(IServiceProvider serviceProvider,
                           ILogger<TelegramBotView> logger,
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


    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        try
        {
            await HandleRequest(botClient, update, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(1000001), e, "Обработка сообщения из телеграмм не удалась. Вероятнее всего проблема в отправке ответа");
        }
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var gptTelegramBotBuilder = scope.ServiceProvider.GetRequiredService<IGptTelegramBotBuilder>();
        await gptTelegramBotBuilder.Build();

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(1000, ct);
        }
    }


    private async Task HandleRequest(ITelegramBotClient botClient, Update request, CancellationToken ct)
    {
        // TODO [NotImpl] - сделать ограничение на количество запросов от одного пользователя

        using var scope = CreateServiceScope(botClient);

        var chatId = request?.Message?.From?.Id ?? request?.CallbackQuery?.From?.Id;

        var handlingResult = await HandleMessageAndExceptions(request, scope);

        await Render(handlingResult, chatId ?? 0, scope, ct);
    }


    private async Task Render(string handlingResult, long chatId, IServiceScope scope, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(handlingResult) || chatId == default)
            return;

        var telegramBotMessageSender = scope.ServiceProvider.GetRequiredService<ITelegramBotMessageSender>();

        await SendMessageWithButtons(handlingResult, chatId, telegramBotMessageSender, ct);
    }


    private async Task SendMessageWithButtons(string handlingResult,
                                              long chatId,
                                              ITelegramBotMessageSender telegramBotMessageSender,
                                              CancellationToken ct)
    {
        var firstButtonLine = new List<KeyboardButton>()
        {
            // new KeyboardButton(TelegramBotButtons.Explain),
        };

        var secondButtonLine = new KeyboardButton[]
        {
            // new KeyboardButton(TelegramBotButtons.OfficialStyle),
            // new KeyboardButton(TelegramBotButtons.TeenageStyle),
        };

        var markup = new ReplyKeyboardMarkup(new[]
        {
            firstButtonLine.ToArray(),
            secondButtonLine
        })
        {
            ResizeKeyboard = true
        };

        var markupedHandlingResult = $"`{handlingResult}`"; // чтобы весь текст мог копироваться и игнор таких проблем, типо не экранированных точек

        await telegramBotMessageSender.Send(markupedHandlingResult, chatId, markup, ct);
    }


    private IServiceScope CreateServiceScope(ITelegramBotClient botClient)
    {
        var scope = _serviceProvider.CreateScope();

        scope.ServiceProvider.GetRequiredService<ITelegramBotClientProvider>().Init(botClient);

        return scope;
    }


    private async Task<string> HandleMessageAndExceptions(Update update, IServiceScope scope)
    {
        var filtersHandlerChain = scope.ServiceProvider.GetRequiredService<IFiltersHandlerChain>();
        var domainEventDistributor = scope.ServiceProvider.GetRequiredService<IPublisher>();
        var telegramBotViews = scope.ServiceProvider.GetServices<ITelegramBotView>().ToList();
        var messageSender = scope.ServiceProvider.GetRequiredService<ITelegramBotMessageSender>();

        try
        {
            var cancellationTokenSource = await ActivateLoadingAnimation(messageSender, update?.Message?.Chat?.Id);

            var result = await RouteMessageOrThrow(update, telegramBotViews);

            DeactivateLoadingAnimation(cancellationTokenSource);

            return result;
        }
        catch (Exception e)
        {
            await domainEventDistributor.Publish(new HandleMessageFailedEvent(update, e));

            return await filtersHandlerChain.FilterException(e);
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

    private async Task<string> RouteMessageOrThrow(Update update, List<ITelegramBotView> telegramBotViews)
    {
        T GetView<T>() where T : ITelegramBotView
        {
            var view = telegramBotViews.OfType<T>().FirstOrDefault();

            return view is null ? throw new InvalidOperationException($"Не найден обработчик для сообщения типа {typeof(T).Name}") : view;
        }

        if (update is null)
            return string.Empty;

        // translation
        if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
        {
            var messageText = update?.Message?.Text;

            return messageText switch
            {

            };
        }

        // /start /block etc
        if (update.Type == UpdateType.MyChatMember
            && update?.MyChatMember?.NewChatMember?.Status != update?.MyChatMember?.OldChatMember?.Status)
        {
            if (update?.MyChatMember?.NewChatMember?.Status == ChatMemberStatus.Kicked)
            {
                // return await GetView<BlockButtonView>().Render(update);
            }

            if (update?.MyChatMember?.NewChatMember?.Status == ChatMemberStatus.Member)
            {
                return string.Empty; // из-за особенностей телеграм ответ в секции "translation", реакция на TelegramBotButtons.Start -> StartButtonView
            }
        }

        // audio msg
        if (update?.Type == UpdateType.Message && update.Message?.Type == MessageType.Voice)
        {
            throw new VoiceMessageTypeNotImplementedException();
        }

        throw new UnknownMessageTypeException();
    }
}

