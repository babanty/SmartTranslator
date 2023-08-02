using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View
{
    public interface ITelegramBotMessageSender // TODO: move to management
    {
        Task<Message> Send(string message, long chatId, IReplyMarkup? keyboard = null, CancellationToken ct = default);

        Task<Message> SendOrUpdateMessage(string message, long chatId, int? MessageId = null, CancellationToken ct = default);

        Task DeleteMessage(long chatId, int messageId, CancellationToken ct = default);
    }
}