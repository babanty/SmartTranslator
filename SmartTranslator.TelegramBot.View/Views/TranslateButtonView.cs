using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class TranslateButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _languageTranslatorController;
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public TranslateButtonView(CoupleLanguageTranslatorController languageTranslatorController,
                               ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _languageTranslatorController = languageTranslatorController;
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("TranslateButtonView got incorrect update (Message == null)");

        await _languageTranslatorController.FinishLatestTranslation(update);
        var text = await _templateStringService.GetSingle("Please, write the text that needs to be translated.");

        return new MessageView
        {
            Text = text.Format(new List<KeyAndNewValue>()),
            Markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
            {
                ResizeKeyboard = true
            }
        };
    }
}
