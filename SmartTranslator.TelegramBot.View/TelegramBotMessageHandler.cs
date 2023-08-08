﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartTranslator.TelegramBot.Management.GptTelegramBots;
using SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;
using SmartTranslator.TelegramBot.View.Views;
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
        try
        {
            await HandleRequest(botClient, update, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(new EventId(1000001), e, "Обработка сообщения из телеграмм не удалась. Вероятнее всего проблема в отправке ответа");
        }
    }


    private async Task HandleRequest(ITelegramBotClient botClient, Update request, CancellationToken ct)
    {
        // TODO [NotImpl] - сделать ограничение на количество запросов от одного пользователя

        using var scope = CreateServiceScope(botClient);

        var chatId = request?.Message?.From?.Id ?? request?.CallbackQuery?.From?.Id;

        var handlingResult = await HandleMessageAndExceptions(request, scope);
        var handlingResultText = handlingResult.Text;

        await Render(handlingResultText, chatId ?? 0, scope, ct, handlingResult);
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


    private async Task<MessageView?> HandleMessageAndExceptions(Update update, IServiceScope scope)
    {
        var filtersHandlerChain = scope.ServiceProvider.GetRequiredService<IFiltersHandlerChain>();
        var domainEventDistributor = scope.ServiceProvider.GetRequiredService<IPublisher>();
        var telegramBotViews = scope.ServiceProvider.GetServices<ITelegramBotView>().ToList();
        var messageSender = scope.ServiceProvider.GetRequiredService<ITelegramBotMessageSender>();
        var router = scope.ServiceProvider.GetRequiredService<TelegramBotRoutingResolver>();
        var loadingAnimator = scope.ServiceProvider.GetRequiredService<ILoadingAnimation>();

        try
        {
            var cancellationTokenSource = await loadingAnimator.ActivateLoadingAnimation(messageSender, update?.Message?.Chat?.Id);

            var view = await router.RouteMessageOrThrow(update, telegramBotViews);
            var result = view is null ? null : await view.Render(update);

            loadingAnimator.DeactivateLoadingAnimation(cancellationTokenSource);

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
}

