using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.Contracts.Requests;

public record CreateTelegramTranslationEntityRequest
{
    public long ChatId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string BaseText { get; set; } = default!;
    public Language? LanguageFrom { get; set; } = default!;
    public Language? LanguageTo { get; set; } = default!;
    public TranslationStyle? TranslationStyle { get; set; } = default!;
}