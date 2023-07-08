using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public interface IGptTranslationManager
{
    /// <summary> Обычный перевод </summary>
    /// <param name="text"> переводимый текст </param>
    /// <param name="translationStyle"> стиль перевода </param>
    /// <param name="languageTo"> Язык на который надо перевести. Если null, то определится автоматически </param>
    /// <param name="context"> Контекст, который передается в promt </param>
    Task<string> Translate(string text, string context, TranslationStyle translationStyle = TranslationStyle.СonversationalStyle, Language? languageTo = null);
}
