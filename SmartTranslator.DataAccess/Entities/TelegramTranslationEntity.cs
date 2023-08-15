using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.DataAccess.Entities;

/// <summary> One specific translation from a specific user in a specific chat  </summary>
public record TelegramTranslationEntity
{
    /// <summary> Id of the translation </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary> Unique user Id in Telegram (also their username) </summary>
    public string UserName { get; set; } = default!;

    /// <summary> Id of the chat where the translation resides </summary>
    public long ChatId { get; set; }

    /// <summary> Creation date (UTC) </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary> Date of last update or creation of the translation entity (UTC) </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary> Base text that was sent for translation </summary>
    public string BaseText { get; set; } = default!;

    /// <summary> The actual translation </summary>
    public string Translation { get; set; } = default!;

    /// <summary> Language from which the translation was made </summary>
    public Language LanguageFrom { get; set; }

    /// <summary> Language to which the translation was made </summary>
    public Language LanguageTo { get; set; }

    /// <summary> Which translation style was used </summary>
    public TranslationStyle TranslationStyle { get; set; }

    /// <summary> Contexts of the translation </summary>
    public IReadOnlyCollection<Context> Contexts { get; set; } = new List<Context>();
}
