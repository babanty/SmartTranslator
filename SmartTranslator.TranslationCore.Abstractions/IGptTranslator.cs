using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;

namespace SmartTranslator.TranslationCore.Abstractions;

/// <summary>
/// GPT-based translator.
/// </summary>
public interface IGptTranslator
{
    /// <summary>
    /// Translates the given text from one language to another.
    /// </summary>
    /// <param name="text">The text to be translated.</param>
    /// <param name="context">The context of the translation (optional).</param>
    /// <param name="from">The source language of the given text.</param>
    /// <param name="to">The target language to which the text will be translated.</param>
    /// <param name="translationStyle">The style of translation to be applied (optional). Defaults to ConversationalStyle.</param>
    /// <returns>Translated text.</returns>
    /// <exception cref="TextIsTooLongException"></exception>
    Task<string> Translate(string text, string? context, Language from, Language to, TranslationStyle translationStyle = TranslationStyle.ConversationalStyle);


    /// <summary>
    /// Evaluates whether there is enough context to unequivocally translate the given text.
    /// </summary>
    /// <param name="text">The text to be evaluated.</param>
    /// <param name="to">The language to which text will be translated.</param>
    /// <returns>Probability of right translation. If it is low, also returns a clarifying question.</returns>
    Task<EvaluationResponse> EvaluateContext(string text, Language to);


    /// <summary>
    /// Defines the style of the input text.
    /// </summary>
    /// <param name="text">The text for which the style is to be defined.</param>
    /// <param name="context">The context of the text (optional).</param>
    /// <param name="from">The source language of the input text (optional).</param>
    /// <param name="to">The target language to which the text will be translated (optional).</param>
    /// <returns><see cref="StyleDefinitionResult"/> with probabilities of correspondance of the given text to all known styles.</returns>
    /// <exception cref="EmptyTextException"></exception>
    /// <exception cref="TextIsTooLongException"></exception>
    /// <exception cref="StyleDefinitionErrorException"></exception>
    Task<StyleDefinitionResult> DefineStyle(string text, string? context, Language? from, Language? to);
}
