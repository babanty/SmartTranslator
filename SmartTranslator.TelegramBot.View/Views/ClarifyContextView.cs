using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class ClarifyContextView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public ClarifyContextView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("ClarifyContextView got incorrect update (Message == null)");

        var question = (await _coupleLanguageTranslatorController.GetLatestContext(update)).Question;
        var templateText = await _templateStringService.GetSingle("Not enough context provided, please, answer the following quesion");
        var text = templateText.Format(new List<KeyAndNewValue>());
        text += Environment.NewLine;
        text += question;


        return new MessageView
        {
            Text = text,
            Markup = new ReplyKeyboardMarkup(new KeyboardButton(TelegramBotButtons.Translate))
        };
    }
}
