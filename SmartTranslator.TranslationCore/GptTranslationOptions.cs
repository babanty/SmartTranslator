namespace SmartTranslator.TranslationCore;

public record GptTranslationOptions
{
    public string ApiKey { get; set; } = default!;

    public int MaxTokens { get; set; } = default!;

    public int MaxSymbols => (int)(MaxTokens * TokenToSymbolsMultiplier);

    /// <summary> A multiplier for converting tokens to symbols. To get the amount of symbols multiply the amount of tokens by this multiplier. </summary>
    private const double TokenToSymbolsMultiplier = 0.25;
}
