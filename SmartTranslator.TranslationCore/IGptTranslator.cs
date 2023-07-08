using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public interface IGptTranslator
{
    /// <exception cref="TextIsTooLongException"/>
    /// <exception cref="FailedToTranslateException/">
    Task<TranslationResult> Translate(string text, Language from, Language to, TranslationStyle translationStyle);

    /// <exception cref="TextIsTooLongException"/>
    /// <exception cref="FailedToTranslateException/">
    Task<TranslationResult> Translate(string text, (Language, Language) couple, TranslationStyle translationStyle);
}
