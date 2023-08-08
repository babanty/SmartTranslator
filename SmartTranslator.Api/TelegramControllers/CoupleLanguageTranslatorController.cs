using Telegram.Bot.Types;

namespace SmartTranslator.Api.TelegramControllers;

public class CoupleLanguageTranslatorController
{
    public async Task NewUser(ChatMemberUpdated chatMemberUpdated)
    {
        return;
    }

    public Task<string> Translate(Message message)
    {
        return Task.FromResult("I don't know how to translate yet");
    }
}
