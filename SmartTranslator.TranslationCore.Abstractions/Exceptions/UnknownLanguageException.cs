namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class UnknownLanguageException : Exception
{
    public UnknownLanguageException(string? message) : base(message)
    {
    }
}
