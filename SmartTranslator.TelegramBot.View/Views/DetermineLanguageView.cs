using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class DetermineLanguageView : ITelegramBotView
{
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public DetermineLanguageView(ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("DetermineLanguageView got incorrect update (Message == null)");

        var text = await _templateStringService.GetSingle("Failed to determine request language, please choose one of the options provided");

        var languageButtons = (new TelegramBotLanguageButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        var translateButton = new KeyboardButton(TelegramBotButtons.Translate);
        var keyboard = new ReplyKeyboardMarkup(new[] { languageButtons, new[] { translateButton } }) { ResizeKeyboard = true };


        return new MessageView
        {
            Text = text.Format(new List<KeyAndNewValue>()),
            Markup = keyboard
        };
    }
}
