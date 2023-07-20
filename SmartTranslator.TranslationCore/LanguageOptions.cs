using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public record LanguageOptions
{
    public Language From { get; set; } = default!;
    public Language To { get; set; } = default!;
}
