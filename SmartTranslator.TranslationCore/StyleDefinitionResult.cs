namespace SmartTranslator.TranslationCore;

public record StyleDefinitionResult
{
    public List<StyleProbability> ProbabilityOfSuccess { get; set; } = default!;
}
