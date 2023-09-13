using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;
/// <summary>
/// Returned when bot gets a message while in a "WaitingForTranslation" state
/// </summary>
public class WaitingForTranslationView : ITelegramBotView
{
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public WaitingForTranslationView(ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        var message = await _templateStringService.GetSingle("WaitAMomentPlease");
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
            new KeyboardButton(TelegramBotButtons.Translate)
        })
        {
            ResizeKeyboard = true
        };

        return new MessageView
        {
            Text = message.Format(new List<KeyAndNewValue>()),
            Markup = replyKeyboard
        };
    }
}
