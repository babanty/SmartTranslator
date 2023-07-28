using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore.Abstractions.Models;

public record StyleProbability
{
    public float Probability { get; set; }
    public TranslationStyle Style { get; set; }
}
