namespace SmartTranslator.TranslationCore;

public record EvaluationResponse
{
    public float Percent { get; set; } = default!;
    public ClarificationRequest Request { get; set; } = default!;
}


