namespace SmartTranslator.TranslationCore.Abstractions.Exceptions;

public class TextIsTooLongException : Exception
{
    public int MaxSymbols { get; }

    public int TextLength { get; }

    public TextIsTooLongException(int maxSymbols, int textLength)
    {
        MaxSymbols = maxSymbols;
        TextLength = textLength;
    }
}
