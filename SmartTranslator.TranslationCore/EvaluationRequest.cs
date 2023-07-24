namespace SmartTranslator.TranslationCore;

public record ClarificationRequest
{
    public string ClarifyingQuestion { get; set; } = default!;
}
