using MediatR;
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
    private readonly List<ITelegramBotView> _telegramBotViews;
    private readonly ITelegramBotMessageSender _messageSender;
    private readonly TelegramBotRoutingResolver _router;
    private readonly ILoadingAnimation _loadingAnimator;


    public TelegramIncomingMessageHandler(ITelegramBotMessageSender telegramBotMessageSender,
                                          IFiltersHandlerChain filtersHandlerChain,
                                          IPublisher domainEventDistributor,
                                          IEnumerable<ITelegramBotView> telegramBotViews,
                                          ITelegramBotMessageSender messageSender,
                                          TelegramBotRoutingResolver router,
                                          ILoadingAnimation loadingAnimator)
    {
        _telegramBotMessageSender = telegramBotMessageSender;
        _filtersHandlerChain = filtersHandlerChain;
        _domainEventDistributor = domainEventDistributor;
        _telegramBotViews = telegramBotViews.ToList();
        _messageSender = messageSender;
        _router = router;
        _loadingAnimator = loadingAnimator;
    }


    public async Task HandleRequest(ITelegramBotClient botClient, Update request, CancellationToken ct)
    {
        // TODO [NotImpl] - сделать ограничение на количество запросов от одного пользователя

        var chatId = request?.Message?.From?.Id ?? request?.CallbackQuery?.From?.Id;

        var handlingResult = await HandleMessageAndExceptions(request);
        var handlingResultText = handlingResult.Text;

        await Render(handlingResultText, chatId ?? 0, ct, handlingResult);
    }


    private async Task Render(string handlingResult, long chatId, CancellationToken ct, MessageView messageView)
    {
        if (string.IsNullOrEmpty(handlingResult) || chatId == default)
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

        var markupedHandlingResult = $"`{text}`"; // чтобы весь текст мог копироваться и игнор таких проблем, типо не экранированных точек

        await telegramBotMessageSender.Send(markupedHandlingResult, chatId, markup, ct);
    }


    private async Task<MessageView?> HandleMessageAndExceptions(Update update)
    {
        try
        {
            var cancellationTokenSource = await _loadingAnimator.ActivateLoadingAnimation(_messageSender, update?.Message?.Chat?.Id);

            var view = await _router.RouteMessageOrThrow(update, _telegramBotViews);
            var result = view is null ? null : await view.Render(update);

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
