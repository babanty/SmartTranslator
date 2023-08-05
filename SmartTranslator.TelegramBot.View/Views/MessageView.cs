using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class MessageView
{
    public string Text { get; set; } = default!;

    public ReplyKeyboardMarkup? Markup = default!;
}