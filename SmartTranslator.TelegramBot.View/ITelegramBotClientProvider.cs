using Telegram.Bot;

namespace SmartTranslator.TelegramBot.View;

/// <summary> Отдает инстанс ITelegramBotClient-а, если он был инициализирован в этом DI scope-e </summary>
public interface ITelegramBotClientProvider // TODO: move to management
{
    public ITelegramBotClient? Instance { get; }

    public ITelegramBotClient GetInstanceOrThrow();

    public void Init(ITelegramBotClient client);
}
