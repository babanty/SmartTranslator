namespace SmartTranslator.Infrastructure.Exceptions;

public class JsonNotFoundException : Exception
{
    public JsonNotFoundException(string? message) : base(message)
    {
    }
}