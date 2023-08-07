using SmartTranslator.TelegramBot.View.Views;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View;

/// <summary> Обрабатывает сообщения из телеграм бота согласно сигнатуре методов-eventHandler-ов </summary>
public interface IGptTelegramBotMessageHandler // TODO: move to management
{
    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}