namespace SmartTranslator.TranslationCore;

public record EvaluationResponse
{
    public int Percent { get; set; } = default!;
    public EvaluationRequest Request { get; set; } = default!;
}


