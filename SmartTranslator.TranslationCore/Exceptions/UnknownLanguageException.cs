namespace SmartTranslator.TranslationCore.Exceptions;

public class UnknownLanguageException : Exception
{
    public UnknownLanguageException(string? message) : base(message)
    {
    }
}
