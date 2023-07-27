using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public record ProbabilityOfSuccessModel
{
    public float Probability { get; set; }
    public TranslationStyle Style { get; set; }
}