using Telegram.Bot;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <summary> Creates client's telegram bot </summary>
public interface IGptTelegramBotBuilder 
{
    Task<(TelegramBotClient BotClient, CancellationTokenSource StopToken)> Build();
}