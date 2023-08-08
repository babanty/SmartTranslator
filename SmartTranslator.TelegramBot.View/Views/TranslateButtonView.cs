using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class TranslateButtonView : ITelegramBotView
{
    public Task<MessageView> Render(Update update)
    {
        var text = $"Была нажата кнопка {TelegramBotButtons.Translate}";

        return Task.FromResult(new MessageView
        {
            Text = text,
            Markup = null
        });
    }
}
