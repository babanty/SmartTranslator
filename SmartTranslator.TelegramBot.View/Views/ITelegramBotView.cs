using TL;

namespace SmartTranslator.TelegramBot.View.Views;

public interface ITelegramBotView
{
    Task<string> Render(Update update);
}
