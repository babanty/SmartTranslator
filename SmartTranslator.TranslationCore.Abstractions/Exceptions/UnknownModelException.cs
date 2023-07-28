namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class UnknownModelException : Exception
{
    public UnknownModelException(string? message) : base(message)
    {
    }
}
