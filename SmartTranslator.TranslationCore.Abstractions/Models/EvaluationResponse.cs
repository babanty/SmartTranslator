namespace SmartTranslator.TranslationCore.Abstractions.Models;

public record EvaluationResponse
{
    public float Percent { get; set; } = default!;
    public ClarificationRequest Request { get; set; } = default!;
}
