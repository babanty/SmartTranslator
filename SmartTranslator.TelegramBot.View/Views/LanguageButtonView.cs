using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class LanguageButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TranslationViewProvider _viewProvider;

    public LanguageButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController, 
                              TranslationViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("LanguageButtonView got incorrect update (Message == null)");
        if (update.Message?.Text == null)
            throw new ArgumentException("LanguageButtonView got incorrect update (Text == null)");

        var language = StringToLanguage(update.Message.Text);

        var dto = await _coupleLanguageTranslatorController.SetLanguages(update, language);

        return await _viewProvider.GetTranslationView(dto).Render(update);
    }


    private static Language StringToLanguage(string text)
    {
        return text switch
        {
            TelegramBotLanguageButtons.English => Language.English,
            TelegramBotLanguageButtons.Russian => Language.Russian,
            "Other" => Language.Unknown,
            _ => throw new UnknownLanguageException($"Unknown language: {text}"),
        };
    }
}
