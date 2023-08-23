using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;
/// <summary>
/// Returned when bot gets a message while in a "WaitingForTranslation" state
/// </summary>
public class WaitingForTranslationView : ITelegramBotView
{
    private readonly ITemplateStringService _templateStringService;

    public WaitingForTranslationView(ITemplateStringService templateStringService)
    {
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        // TODO: add such a template to service
        var message = await _templateStringService.GetSingle("WaitAMomentPlease");
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
            new KeyboardButton(TelegramBotButtons.Translate)
        });

        return new MessageView
        {
            Text = message.Format(new List<KeyAndNewValue>()),
            Markup = replyKeyboard
        };
    }
}
