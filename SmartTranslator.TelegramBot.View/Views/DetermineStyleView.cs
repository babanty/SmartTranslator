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

        var styleButtons = (new TelegramBotStyleButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        
        int halfCount = (styleButtons.Length + 1) / 2; // If number of buttons is odd, there will be more buttons in the first row
        
        var firstRowButtons = styleButtons.Take(halfCount).Select(button => button).ToArray();
        var secondRowButtons = styleButtons.Skip(halfCount).Select(button => button).ToArray();
        var translateButtonRow = new[] { new KeyboardButton(TelegramBotButtons.Translate) };

        var keyboard = new ReplyKeyboardMarkup(new[] { firstRowButtons, secondRowButtons, translateButtonRow }) { ResizeKeyboard = true };

        return new MessageView
        {
            Text = text.Format(new List<KeyAndNewValue>()),
            Markup = keyboard
        };
    }
}
