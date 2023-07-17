using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public class LanguageManager : ILanguageManager
{
    private const int _sendAttemptsCount = 5;
    private readonly LanguageOptions _languageOptions;
    private readonly GptHttpClient _gptHttpClient;

    public LanguageManager(LanguageOptions options, GptHttpClient httpClient)
    {
        _languageOptions = options;
        _gptHttpClient = httpClient;
    }

    public (Language, Language) GetLanguagePair()
    {
        return (_languageOptions.From, _languageOptions.To);
    }


    public async Task<Language> DetermineLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Language.Unknown;
        }

        if (text.Length > 50)
        {
            text = text[..50];
        }

        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser($"What is the language of the text: \"{text}\". Language should be: eng, rus, other")
        };

        var translation = await _gptHttpClient.Send(messages, GptModel.GPT3d5Stable, _sendAttemptsCount);

        return StringToLanguage(translation);
    }


    private static Language StringToLanguage(string text)
    {
        return text switch
        {
            "eng" => Language.English,
            "rus" => Language.Russian,
            "other" => Language.Unknown,
            _ => throw new UnknownLanguageException($"Unknown language: {text}"),
        };
    }
}
