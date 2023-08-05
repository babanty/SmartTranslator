using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public interface ITelegramBotView
{
    Task<MessageView> Render(Update update);
}
