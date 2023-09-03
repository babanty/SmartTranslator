using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class TranslateButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _languageTranslatorController;

    public TranslateButtonView(CoupleLanguageTranslatorController languageTranslatorController)
    {
        _languageTranslatorController = languageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("TranslateButtonView got incorrect update (Message == null)");

        await _languageTranslatorController.FinishLatestTranslation(update);
        var text = $"Please, write the text that needs to be translated.";

        return new MessageView
        {
            Text = text,
            Markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
        };
    }
}
