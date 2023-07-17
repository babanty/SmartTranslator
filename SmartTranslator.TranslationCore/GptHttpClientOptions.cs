namespace SmartTranslator.TranslationCore;

public record GptHttpClientOptions
{
    public string ApiKey { get; set; } = default!;
}
