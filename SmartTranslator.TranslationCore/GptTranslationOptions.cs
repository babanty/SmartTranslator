using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore;

public record GptTranslationOptions
{
    public int MaxTokens { get; set; } = default!;

    public CoupleLanguageHolder CoupleLanguage { get; set; } = default!;

    public int MaxSymbols => (int)(MaxTokens * TokenToSymbolsMultiplier);

    /// <summary> A multiplier for converting tokens to symbols. To get the amount of symbols multiply the amount of tokens by this multiplier. </summary>
    private const double TokenToSymbolsMultiplier = 0.25;

    public record CoupleLanguageHolder
    {
        public Language Item1 { get; set; } = default!;
        public Language Item2 { get; set; } = default!;

        /// <summary> example: 🧠 </summary>
        public string Item1Ico { get; set; } = default!;

        /// <summary> example: 🧠 </summary>
        public string Item2Ico { get; set; } = default!;
    }
}
