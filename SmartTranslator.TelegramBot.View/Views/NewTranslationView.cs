using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class NewTranslationView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITranslationManager _translationManager;

    public NewTranslationView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              ITranslationManager translationManager)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _translationManager = translationManager;
    }


    public async Task<MessageView> Render(Update update)
    {
        var translation = await _coupleLanguageTranslatorController.CreateTranslation();
        // Fill the properties
        return  translation.State switch
        {
            var state when state == TelegramTranslationState.WaitingForTranslation => GetView<>()
        }
    }
}