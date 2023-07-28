namespace SmartTranslator.TranslationCore;

public interface ITextMistakeManager
{
    Task<string> Correct(string text);
}
