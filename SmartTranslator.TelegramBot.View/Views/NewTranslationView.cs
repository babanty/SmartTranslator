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
    private readonly TranslationViewProvider _viewProvider;

    public NewTranslationView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              TranslationViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        var translation = await _coupleLanguageTranslatorController.CreateTranslation(update);
        // Fill the properties
        return await _viewProvider.GetTranslationView(translation).Render(update);
    }
}