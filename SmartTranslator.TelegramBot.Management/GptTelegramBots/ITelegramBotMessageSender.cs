using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

public interface ITelegramBotMessageSender
{
    Task<Message> Send(string message, long chatId, IReplyMarkup? keyboard = null, CancellationToken ct = default);

    Task<Message> SendOrUpdateMessage(string message, long chatId, int? MessageId = null, CancellationToken ct = default);

    Task DeleteMessage(long chatId, int messageId, CancellationToken ct = default);
}