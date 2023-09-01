using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;
using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore;

public class LanguageManager : ILanguageManager
{
    private readonly LanguageOptions _languageOptions;
    private readonly IGptHttpClient _gptHttpClient;

    public LanguageManager(LanguageOptions options, IGptHttpClient httpClient)
    {
        _languageOptions = options;
        _gptHttpClient = httpClient;
    }


    /// <inheritdoc/>
    public (Language, Language) GetLanguagePair() => (_languageOptions.From, _languageOptions.To);


    /// <inheritdoc/>
    public async Task<Language?> DetermineLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Language.Unknown;
        }

        var textToCheck = text.Length > 50 ? text.Substring(0, 50) : text;
        var possibleLanguages = GetLanguagePair();

        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser($"What is the language of the text: \"{textToCheck}\"? As your answer only say one of the following words: {possibleLanguages.Item1}, {possibleLanguages.Item2}, null")
        };

        var translation = await _gptHttpClient.Send(messages, GptModel.GPT3d5Stable);

        return StringToLanguage(translation);
    }


    private static Language? StringToLanguage(string text)
    {
        return text.ToLowerInvariant() switch
        {
            var t when t.Contains("english") => Language.English,
            var t when t.Contains("russian") => Language.Russian,
            var t when t.Contains("null") => null,
            _ => throw new UnknownLanguageException($"Unknown language: {text}")
        };
    }
}
