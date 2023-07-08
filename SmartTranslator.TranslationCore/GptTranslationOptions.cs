using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public record GptTranslationOptions
{
    public string ApiKey { get; set; } = default!;

    public int MaxTokens { get; set; } = default!;

    public CoupleLanguageHolder CoupleLanguage { get; set; } = default!;

    public int MaxSymbols => (int)(MaxTokens * TokenToSymbolsMultiplier);


    /// <summary> Перевод токенов в символы, множитель. Чтобы получить количество символов надо умножить количество токенов на этот множитель </summary>
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
