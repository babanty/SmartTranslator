using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class TooShortRequestView : ITelegramBotView
{
    public async Task<MessageView> Render(Update update)
    {
        return new MessageView
        {
            Text = "Your request was too short, it must be 5 characters or more.", // TODO: make multilingual
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
