using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <inheritdoc/>
public class TelegramBotMessageSender : ITelegramBotMessageSender
{
    private ITelegramBotClient _botClient => _telegramBotClientProvider.GetInstanceOrThrow();

    private readonly ITelegramBotClientProvider _telegramBotClientProvider;
    private readonly ILogger<TelegramBotMessageSender> _logger;

    public TelegramBotMessageSender(ITelegramBotClientProvider telegramBotClientProvider,
                                    ILogger<TelegramBotMessageSender> logger)
    {
        _telegramBotClientProvider = telegramBotClientProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Message> Send(string message, long chatId, IReplyMarkup? keyboard = null, CancellationToken ct = default)
    {
        Message sentMessage = await _botClient.SendTextMessageAsync(
               chatId: chatId,
               text: message,
               parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
               replyMarkup: keyboard,
               cancellationToken: ct);

        return sentMessage;
    }

    /// <inheritdoc/>
    public async Task<Message> SendOrUpdateMessage(string message, long chatId, int? messageId = null, CancellationToken ct = default)
    {
        if (!messageId.HasValue)
        {
            return await Send(message, chatId, null, ct);
        }

        Message sentMessage;

        try
        {
            sentMessage = await _botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId.Value,
                text: message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                cancellationToken: ct);
        }
        catch (ApiRequestException ex)
        {
            if (ex.Message.Contains("message identifier is not"))
            {
                sentMessage = await Send(message, chatId, null, ct);
            }
            else
            {
                throw;
            }
        }

        return sentMessage;
    }

    /// <inheritdoc/>
    public async Task DeleteMessage(long chatId, int messageId, CancellationToken ct = default)
    {
        try
        {
            await _botClient.DeleteMessageAsync(chatId, messageId, ct);
        }
        catch (ApiRequestException ex)
        {
            _logger.LogWarning(new EventId(675830436), ex, "Failed to delete message from chat {chatId}", chatId);
        }
    }

}
