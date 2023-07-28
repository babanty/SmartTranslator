namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class StyleDefinitionErrorException : Exception
{
    public StyleDefinitionErrorException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
