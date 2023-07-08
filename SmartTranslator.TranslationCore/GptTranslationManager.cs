using SmartTranslator.Enums;
using Microsoft.Extensions.Options;

namespace SmartTranslator.TranslationCore;

public class GptTranslationManager : IGptTranslationManager
{
    private readonly IGptTranslator _gptTranslator;
    private readonly GptTranslationOptions _options;


    public GptTranslationManager(IGptTranslator gptTranslator, IOptions<GptTranslationOptions> options)
    {
        _gptTranslator = gptTranslator;
        _options = options.Value ?? new();
    }


    /// <inheritdoc/>
    public async Task<string> Translate(string text, string context, TranslationStyle translationStyle = TranslationStyle.СonversationalStyle, Language? languageTo = null)
    {
        if (languageTo is null)
        {
            return (await _gptTranslator.Translate(text, (_options.CoupleLanguage.Item1, _options.CoupleLanguage.Item2), translationStyle)).Translation;
        }

        var languages = _options.CoupleLanguage;
        var languageFrom = languageTo == languages.Item1 ? languages.Item2 : languages.Item1;

        return (await _gptTranslator.Translate(text, languageFrom, languageTo.Value, translationStyle)).Translation;
    }
}
