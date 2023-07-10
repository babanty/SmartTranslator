namespace SmartTranslator.TranslationCore.Exceptions;

public class FailedToTranslateException : Exception
{
    public FailedToTranslateException(string msg) : base(msg)
    {
    }
}
