using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TelegramBot.View.Exceptions;
using SmartTranslator.TelegramBot.View.Views;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotRoutingResolver
{
    private readonly ITranslationManager _translationManager;

    public TelegramBotRoutingResolver(ITranslationManager translationManager)
    {
        _translationManager = translationManager;
    }


    public async Task<ITelegramBotView?> RouteMessageOrThrow(Update update, List<ITelegramBotView> telegramBotViews)
    {
        T GetView<T>() where T : ITelegramBotView
        {
            var view = telegramBotViews.OfType<T>().FirstOrDefault();

            return view is null ? throw new InvalidOperationException($"No handler found for a message of type {typeof(T).Name}") : view;
        }

        if (update is null)
            return null;

        // translation
        if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text && update.Message?.Text != null)
        {
            var messageText = update?.Message?.Text;

            return messageText switch
            {
                var text when text == TelegramBotButtons.Start => GetView<StartButtonView>(),
                var text when text == TelegramBotButtons.Translate => GetView<TranslateButtonView>(),
                var text when text == "Determine language" => GetView<DetermineLanguageView>(),
                var text when text == "Clarify" => GetView<ClarifyContextView>(),
                var text when text == "Determine style" => GetView<DetermineStyleView>(),
                var text when text == "Answer" => GetView<FinalAnswerView>(),
                var text when IsCertainButtonType(text!, new TelegramBotLanguageButtons()) => GetView<LanguageButtonView>(),
                var text when IsCertainButtonType(text!, new TelegramBotStyleButtons()) => GetView<StyleButtonView>(),
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
                return GetView<ChangedStatusToMemberView>();
            }
        }

        // audio msg
        if (update?.Type == UpdateType.Message && update.Message?.Type == MessageType.Voice)
        {
            throw new VoiceMessageTypeNotImplementedException();
        }

        if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text && update.Message?.Text != null)
        {
            var latest = await _translationManager.GetLatest(update.Message.From.Username, update.Message.Chat.Id);

            return latest.State switch
            {
                var state when state == TelegramTranslationState.Finished => GetView<NewTranslationView>(),
            };
        }

        // Получаем последний переведённый текст +
        // Проверяем заполненность текста! +
        // Если текст.статус == завершен, тогда возвращать вьюшку new tranlation, которая вызывает метод контроллера create и получает результат
        //          Результат - translation, в зависимости от статуса получает новую view и вызывает метод render()
        //          Возможные варианты получаемых view в зависимости от статуса: выбор языка, стиля, контекста, перевода
        // Если текст.статус == ожидаемКонтекст, тогда вернуть вьюшку FillContext, который возвращает последний активный вопрос о контексте
        // Если текст.статус == 
        //var text when(await _translationManager.GetLatest(update.Message.From.Username, update.Message.Chat.Id)).State == TelegramTranslationState.WaitingForContext => GetView<ClarifyContextView>(),


        throw new UnknownMessageTypeException();
    }

    /// <summary>
    /// Checks whether provided text is one of given type of buttons
    /// </summary>
    private bool IsCertainButtonType(string text, IButtonsHolder holder) => holder.Buttons.Contains(text);
}
