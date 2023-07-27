namespace SmartTranslator.TranslationCore;

public interface ISpellingManager
{
    Task<string> CorrectSpelling(string text);
}
