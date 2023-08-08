using SmartTranslator.TelegramBot.Management.GptTelegramBots;

namespace SmartTranslator.TelegramBot.View;

public interface ILoadingAnimation
{
    Task<CancellationTokenSource> ActivateLoadingAnimation(ITelegramBotMessageSender messageSender, long? chatId);

    void DeactivateLoadingAnimation(CancellationTokenSource cancellationTokenSource);
}
