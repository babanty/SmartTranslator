namespace SmartTranslator.TranslationCore.Abstractions.Models;

public record ClarificationRequest
{
    public string ClarifyingQuestion { get; set; } = default!;
}
