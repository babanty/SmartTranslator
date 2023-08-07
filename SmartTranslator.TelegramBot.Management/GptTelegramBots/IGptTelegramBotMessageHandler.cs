using Telegram.Bot;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <summary> Processes messages from the Telegram bot according to the signature of the method's event handlers. </summary>
public interface IGptTelegramBotMessageHandler 
{
    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}