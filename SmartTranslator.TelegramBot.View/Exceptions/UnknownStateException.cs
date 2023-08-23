namespace SmartTranslator.TelegramBot.View.Exceptions;

public class UnknownStateException : Exception
{
    public UnknownStateException(string? message) : base(message)
    {
    }
}
