namespace SmartTranslator.TranslationCore.Abstractions.Models;

public record StyleDefinitionResult
{
    public List<StyleProbability> ProbabilityOfSuccess { get; set; } = default!;
}
