using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TelegramBot.View.Exceptions;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class NewTranslationView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TelegramViewProvider _viewProvider;

    public NewTranslationView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              TelegramViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        var translation = await _coupleLanguageTranslatorController.CreateTranslation(update);
        // Fill the properties
        return translation.State switch
        {
            var state when state == TelegramTranslationState.Finished => await _viewProvider.GetView<TranslateButtonView>().Render(update),
            var state when state == TelegramTranslationState.WaitingForTranslation => await _viewProvider.GetView<TranslateButtonView>().Render(update),
            var state when state == TelegramTranslationState.WaitingForStyle => await _viewProvider.GetView<DetermineStyleView>().Render(update),
            var state when state == TelegramTranslationState.WaitingForContext => await _viewProvider.GetView<ClarifyContextView>().Render(update),
            var state when state == TelegramTranslationState.WaitingForLanguage => await _viewProvider.GetView<DetermineLanguageView>().Render(update),
            _ => throw new UnknownStateException($"Unknown state: {translation.State}")
        };
    }
}