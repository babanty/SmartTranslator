using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class DetermineStyleView : ITelegramBotView
{
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public DetermineStyleView(ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        var text = await _templateStringService.GetSingle("Failed to determine style of request, please choose one of the options provided");

        var languageButtons = (new TelegramBotStyleButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        var translateButton = new KeyboardButton(TelegramBotButtons.Translate);
        var keyboard = new ReplyKeyboardMarkup(new[] { languageButtons, new[] { translateButton } });

        return new MessageView
        {
            Text = text.Format(new List<KeyAndNewValue>()),
            Markup = keyboard
        };
    }
}
