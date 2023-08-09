using Telegram.Bot;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <inheritdoc/>
public class TelegramBotClientProvider : ITelegramBotClientProvider
{
    public ITelegramBotClient? Instance { get; private set; }

    public ITelegramBotClient GetInstanceOrThrow() => Instance
        ?? throw new NullReferenceException("Trying to get ITelegramBotClient before it was initialized in TelegramBotClientProvider");

    public void Init(ITelegramBotClient client) => Instance = client;
}
