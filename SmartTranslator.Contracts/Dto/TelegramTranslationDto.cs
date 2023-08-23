using SmartTranslator.Enums;

namespace SmartTranslator.Contracts.Dto;

public record TelegramTranslationDto
{
    public string Id { get; set; } = default!;
    public TelegramTranslationState State { get; set; } = default!;
}
