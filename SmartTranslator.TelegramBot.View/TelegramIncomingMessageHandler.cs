﻿using MediatR;
using SmartTranslator.TelegramBot.Management.GptTelegramBots;
using SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;
using SmartTranslator.TelegramBot.View.Views;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View;

public class TelegramIncomingMessageHandler
{
    private readonly ITelegramBotMessageSender _telegramBotMessageSender;
    private readonly IFiltersHandlerChain _filtersHandlerChain;
    private readonly IPublisher _domainEventDistributor;
    private readonly ITelegramBotMessageSender _messageSender;
    private readonly TelegramBotRoutingResolver _router;
    private readonly ILoadingAnimation _loadingAnimator;


    public TelegramIncomingMessageHandler(ITelegramBotMessageSender telegramBotMessageSender,
                                          IFiltersHandlerChain filtersHandlerChain,
                                          IPublisher domainEventDistributor,
                                          ITelegramBotMessageSender messageSender,
                                          TelegramBotRoutingResolver router,
                                          ILoadingAnimation loadingAnimator)
    {
        _telegramBotMessageSender = telegramBotMessageSender;
        _filtersHandlerChain = filtersHandlerChain;
        _domainEventDistributor = domainEventDistributor;
        _messageSender = messageSender;
        _router = router;
        _loadingAnimator = loadingAnimator;
    }


    public async Task HandleRequest(ITelegramBotClient botClient, Update request, CancellationToken ct)
    {
        // TODO [NotImpl] - limit the number of requests from a single user

        if ((botClient == null) || (request == null))
        {
            return;
        }

        var chatId = request?.Message?.From?.Id ?? request?.CallbackQuery?.From?.Id;

        var handlingResult = await HandleMessageAndExceptions(request!);

        await Render(handlingResult, chatId ?? 0, ct);
    }


    private async Task Render(MessageView? messageView, long chatId, CancellationToken ct)
    {
        if (messageView == null || chatId == default)
            return;

        await SendMessageWithButtons(chatId, _telegramBotMessageSender, ct, messageView);
    }


    private async Task SendMessageWithButtons(long chatId,
                                          ITelegramBotMessageSender telegramBotMessageSender,
                                          CancellationToken ct,
                                          MessageView messageView)
    {
        var text = messageView.Text;
        var markup = messageView.Markup;
        var inlineMarkup = messageView.InlineMarkup;

        var markupedHandlingResult = $"`{text}`"; // So that all the text can be copied, ignoring such problems, like unescaped dots

        if (inlineMarkup != null)
            await telegramBotMessageSender.Send(markupedHandlingResult, chatId, inlineMarkup, ct);
        else
            await telegramBotMessageSender.Send(markupedHandlingResult, chatId, markup, ct);
    }


    private async Task<MessageView?> HandleMessageAndExceptions(Update update)
    {
        try
        {
            var cancellationTokenSource = await _loadingAnimator.ActivateLoadingAnimation(_messageSender, update?.Message?.Chat?.Id);

            var view = await _router.RouteMessageOrThrow(update!);
            var result = view is null ? null : await view.Render(update!);

            _loadingAnimator.DeactivateLoadingAnimation(cancellationTokenSource);

            return result;
        }
        catch (Exception e)
        {
            await _domainEventDistributor.Publish(new HandleMessageFailedEvent(update, e));

            var message = await _filtersHandlerChain.FilterException(e);
            return new MessageView
            {
                Text = message,
                Markup = null
            };
        }
    }
}
