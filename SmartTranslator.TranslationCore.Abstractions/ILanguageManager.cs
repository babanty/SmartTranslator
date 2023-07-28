using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore.Abstractions;

/// <summary>
/// Language manager that provides methods to work with languages.
/// </summary>
public interface ILanguageManager
{
    /// <summary>
    /// Gets a pair of languages with which the entire app is working with.
    /// </summary>
    /// <returns>
    /// A tuple containing two <see cref="Language"/> objects representing the language pair.
    /// </returns>
    (Language, Language) GetLanguagePair();


    /// <summary>
    /// Determines the language of the given text.
    /// </summary>
    /// <param name="text">Text which language needs to be determined.</param>
    /// <returns>
    ///  <see cref="Language"/> of the given text.
    /// </returns>
    Task<Language> DetermineLanguage(string text);
}
