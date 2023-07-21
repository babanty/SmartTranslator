namespace SmartTranslator.TranslationCore.Tests;

public record IntegrationTestOptions
{
    public string ApiKey { get; set; } = default!;

    public int MaxTokens { get; set; } = default!;
}