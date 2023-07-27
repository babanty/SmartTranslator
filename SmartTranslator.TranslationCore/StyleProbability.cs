using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public record StyleProbability
{
    public float Probability { get; set; }
    public TranslationStyle Style { get; set; }
}