namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class FailedToTranslateException : Exception
{
    public FailedToTranslateException(string msg) : base(msg)
    {
    }
}
