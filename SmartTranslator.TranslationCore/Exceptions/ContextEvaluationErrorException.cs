namespace SmartTranslator.TranslationCore.Exceptions;

public class ContextEvaluationErrorException : Exception
{
    public ContextEvaluationErrorException(string? message) : base(message)
    {
    }
}
