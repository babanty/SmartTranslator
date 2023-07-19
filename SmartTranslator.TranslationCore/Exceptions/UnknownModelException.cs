namespace SmartTranslator.TranslationCore.Exceptions;

public class UnknownModelException : Exception
{
    public UnknownModelException(string? message) : base(message)
    {
    }
}
