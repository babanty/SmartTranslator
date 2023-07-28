namespace SmartTranslator.TranslationCore.Exceptions;

public class StyleDefinitionErrorException : Exception
{
    public StyleDefinitionErrorException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
