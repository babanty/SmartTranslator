using SmartTranslator.Enums;

namespace SmartTranslator.DataAccess.Entities;

public record BotSubscriptionStatistics
{
    public Guid Id { get; set; }

    public DateTime HappennedAt { get; set; }

    public BotSubscriptionActionType ActionType { get; set; }

    /// <summary> Name of the user </summary>
    public string UserName { get; set; } = default!;

    /// <summary> Language pair controlled by one app </summary>
    public string LanguagePair { get; set; } = default!;
}
