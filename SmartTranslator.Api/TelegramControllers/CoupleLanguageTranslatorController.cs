using SmartTranslator.TranslationCore.Enums;
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

    public Task<Language> DetermineLanguage(Message message)
    {
        return Task.FromResult(Language.Unknown);
    }


    public async Task SetLanguage(Language language)
    {
        return;
    }
}
