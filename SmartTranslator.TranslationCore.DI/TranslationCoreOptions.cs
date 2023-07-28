namespace SmartTranslator.TranslationCore.DI;

public record TranslationCoreOptions
{
    public GptHttpClientOptions GptHttpClientOptions { get; set; } = new();
    public GptTranslationOptions GptTranslationOptions { get; set; } = new();
    public LanguageOptions LanguageOptions { get; set; } = new();
}
