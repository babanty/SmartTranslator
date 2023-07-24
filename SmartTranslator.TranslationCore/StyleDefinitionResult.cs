using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore;

public record StyleDefinitionResult 
{
    public List<(float, TranslationStyle)> ProbabilityOfSuccess { get; set; } = default!;
}
