using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public record TranslationResult
{
    public string Translation { get; set; } = default!;

    public Language LanguageTo { get; set; } = default!;
}


public record TranslationResultProvider
{
    private TranslationResult? _result;

    public TranslationResult? Result
    {
        get => _result;
        set
        {
            if (value is null || string.IsNullOrEmpty(value.Translation))
            {
                throw new FailedToTranslateException("Результат перевода - пустое сообщение");
            }
            _result = value;
        }
    }
}
