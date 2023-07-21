namespace SmartTranslator.TranslationCore;

public record EvaluationRequest
{
    public string ClarifyingQuestion { get; set; } = default!;
}
