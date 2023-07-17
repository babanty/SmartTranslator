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
        var from = StringToLanguage(_languageOptions.From);
        var to = StringToLanguage(_languageOptions.To);
        return (from, to);
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
            ChatMessage.FromUser($"What is the language of the text: \"{text}\"? As your answer only say one of the following words: English, Russian, Other")
        };

        var translation = await _gptHttpClient.Send(messages, GptModel.GPT3d5Stable, _sendAttemptsCount);

        return StringToLanguage(translation);
    }


    private static Language StringToLanguage(string text)
    {
        return text switch
        {
            "English" => Language.English,
            "Russian" => Language.Russian,
            "Other" => Language.Unknown,
            _ => throw new UnknownLanguageException($"Unknown language: {text}"),
        };
    }
}
