using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore;


public interface IGptTranslator
{
    /// <summary> Regular translation </summary>
    /// <param name="text"> Text to translate </param>
    /// <param name="context"> Context passed to prompt </param>
    /// <param name="to"> Language to translate to </param>
    /// <param name="from"> Language to translate from </param>
    /// <param name="translationStyle"> Translation style </param>
    Task<string> Translate(string text, string context, Language from, Language to, TranslationStyle translationStyle = TranslationStyle.ConversationalStyle);


    Task<StyleDefinitionResult> DefineStyle(string text, string? context, Language? from, Language? to);
}