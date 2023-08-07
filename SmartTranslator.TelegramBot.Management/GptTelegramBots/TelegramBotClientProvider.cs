using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <inheritdoc/>
public class TelegramBotClientProvider : ITelegramBotClientProvider
{
    public ITelegramBotClient? Instance { get; private set; }

    public ITelegramBotClient GetInstanceOrThrow() => Instance
        ?? throw new NullReferenceException("Пытаемся получить ITelegramBotClient до того как он был инициализирован в TelegramBotClientProvider");

    public void Init(ITelegramBotClient client) => Instance = client;
}
