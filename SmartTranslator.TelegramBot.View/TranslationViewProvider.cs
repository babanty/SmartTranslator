using SmartTranslator.Contracts.Dto;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.View.Exceptions;
using SmartTranslator.TelegramBot.View.Views;

namespace SmartTranslator.TelegramBot.View;

public class TranslationViewProvider
{
    private readonly TelegramViewProvider _viewProvider;

    public TranslationViewProvider(TelegramViewProvider viewProvider)
    {
        _viewProvider = viewProvider;
    }


    public ITelegramBotView GetTranslationView(TelegramTranslationDto translation) => translation.State switch
    {
        var state when state == TelegramTranslationState.Finished => _viewProvider.GetView<FinalAnswerView>(),
        var state when state == TelegramTranslationState.WaitingForTranslation => _viewProvider.GetView<TranslateButtonView>(),
        var state when state == TelegramTranslationState.WaitingForStyle => _viewProvider.GetView<DetermineStyleView>(),
        var state when state == TelegramTranslationState.WaitingForContext => _viewProvider.GetView<ClarifyContextView>(),
        var state when state == TelegramTranslationState.WaitingForLanguage => _viewProvider.GetView<DetermineLanguageView>(),
        _ => throw new UnknownStateException($"Unknown state: {translation.State}")
    };
}
