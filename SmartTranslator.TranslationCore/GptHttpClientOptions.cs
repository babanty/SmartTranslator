namespace SmartTranslator.TranslationCore;

public record GptHttpClientOptions
{
    public string ApiKey { get; set; } = default!;
    public int AttemptsCount { get; set; } = 5;
}
