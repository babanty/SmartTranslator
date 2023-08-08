using Telegram.Bot;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots;

/// <summary> Provides ITelegramBotClient instance, if it was initialized in this DI scope </summary>
public interface ITelegramBotClientProvider
{
    public ITelegramBotClient? Instance { get; }

    public ITelegramBotClient GetInstanceOrThrow();

    public void Init(ITelegramBotClient client);
}
