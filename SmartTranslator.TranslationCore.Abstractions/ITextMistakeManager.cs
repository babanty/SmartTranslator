using SmartTranslator.TranslationCore.Abstractions.Exceptions;

namespace SmartTranslator.TranslationCore.Abstractions;

/// <summary>
/// Manager that corrects all mistakes found in the given text.
/// </summary>
public interface ITextMistakeManager
{
    /// <summary>
    /// Corrects all mistakes found in the given text (spelling, grammatical, punctuational).
    /// </summary>
    /// <param name="text">Given text.</param>
    /// <returns>Corrected text.</returns>
    /// <exception cref="EmptyTextException"></exception>
    /// <exception cref="TextIsTooLongException"></exception>
    /// <exception cref="CorrectionErrorException"></exception>
    Task<string> Correct(string text);
}
