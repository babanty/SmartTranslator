using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.Contracts.Requests;

public record CreateTelegramTranslationEntityRequest
{
    public long ChatId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string BaseText { get; set; } = default!;
}