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
    private readonly TelegramViewProvider _viewProvider;

    public TelegramBotRoutingResolver(ITranslationManager translationManager,
                                      TelegramViewProvider viewProvider)
    {
        _translationManager = translationManager;
        _viewProvider = viewProvider;
    }


    public async Task<ITelegramBotView?> RouteMessageOrThrow(Update update)
    {
        if (update is null)
            return null;

        // translation
        if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text && update.Message?.Text != null)
        {
            var messageText = update?.Message?.Text;

            ITelegramBotView? result = messageText switch
            {
                var text when text == TelegramBotButtons.Start => _viewProvider.GetView<StartButtonView>(),
                var text when text == TelegramBotButtons.Translate => _viewProvider.GetView<TranslateButtonView>(),
                var text when text == "Determine language" => _viewProvider.GetView<DetermineLanguageView>(),
                // TODO: cleanup
                var text when text == "Clarify" => _viewProvider.GetView<ClarifyContextView>(),
                var text when text == "Determine style" => _viewProvider.GetView<DetermineStyleView>(),
                var text when text == "Answer" => _viewProvider.GetView<FinalAnswerView>(),
                var text when IsCertainButtonType(text!, new TelegramBotLanguageButtons()) => _viewProvider.GetView<LanguageButtonView>(),
                var text when IsCertainButtonType(text!, new TelegramBotStyleButtons()) => _viewProvider.GetView<StyleButtonView>(),
                _ => null
            };

            if (result != null)
                return result;
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
                return _viewProvider.GetView<ChangedStatusToMemberView>();
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

            if (latest == null)
                return _viewProvider.GetView<NewTranslationView>();

            return latest.State switch
            {
                var state when state == TelegramTranslationState.Finished => _viewProvider.GetView<NewTranslationView>(),
                var state when state == TelegramTranslationState.WaitingForTranslation => _viewProvider.GetView<WaitingForTranslationView>(),
                var state when state == TelegramTranslationState.WaitingForStyle => _viewProvider.GetView<AddExtraContextInsteadOfStyleView>(),
                var state when state == TelegramTranslationState.WaitingForContext => _viewProvider.GetView<FillContextView>(),
                var state when state == TelegramTranslationState.WaitingForLanguage => _viewProvider.GetView<AddExtraContextInsteadOfLanguageView>(),
                _ => throw new UnknownStateException($"Unknown state: {latest.State}")
            };
        }

        throw new UnknownMessageTypeException();
    }

    /// <summary>
    /// Checks whether provided text is one of given type of buttons
    /// </summary>
    private bool IsCertainButtonType(string text, IButtonsHolder holder) => holder.Buttons.Contains(text);
}
