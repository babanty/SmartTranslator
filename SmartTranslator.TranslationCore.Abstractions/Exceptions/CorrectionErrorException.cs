namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class CorrectionErrorException : Exception
{
    public CorrectionErrorException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
