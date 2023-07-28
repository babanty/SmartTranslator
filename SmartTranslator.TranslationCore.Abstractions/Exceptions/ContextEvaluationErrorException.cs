namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class ContextEvaluationErrorException : Exception
{
    public ContextEvaluationErrorException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
