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

    public LanguageButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        var language = StringToLanguage(update.Message.Text);

        await _coupleLanguageTranslatorController.SetLanguage(language);

        return await Task.FromResult(new MessageView
        {
            Text = $"Language set to {update.Message.Text}",
            Markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
        });
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
