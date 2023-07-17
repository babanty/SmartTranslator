namespace SmartTranslator.TranslationCore;

public record LanguageOptions
{
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
}
