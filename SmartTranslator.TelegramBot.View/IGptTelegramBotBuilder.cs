using Telegram.Bot;

namespace SmartTranslator.TelegramBot.View;

/// <summary> Создает telegram-bot клиента </summary>
public interface IGptTelegramBotBuilder // TODO: move to management
{
    Task<(TelegramBotClient BotClient, CancellationTokenSource StopToken)> Build();
}