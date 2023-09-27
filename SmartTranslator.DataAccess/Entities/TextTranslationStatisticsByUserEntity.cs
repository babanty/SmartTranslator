namespace SmartTranslator.DataAccess.Entities;

/// <summary> Translation statistics of particular user </summary>
public record TextTranslationStatisticsByUserEntity
{
    public Guid Id { get; set; }

    /// <summary> Date of first translation </summary>
    public DateTime FirstAt { get; set; }

    /// <summary> Date of last translation </summary>
    public DateTime LastAt { get; set; }

    /// <summary> Name of the user </summary>
    public string UserName { get; set; } = default!;

    /// <summary> Translation style used </summary>
    public string TranslationStyle { get; set; } = default!;

    /// <summary> Amount of times user successfully used translation </summary>
    public int SuccessfullyUsedTranslationCount { get; set; }

    /// <summary> Average size of successfully translated text </summary>
    public int AverageTextSize { get; set; }

    /// <summary> Language pair controlled by one app </summary>
    public string LanguagePair { get; set; } = default!;

    /// <summary> 
    /// Amount of requests for translation into first language in pair
    /// Amount of requests into second language: SuccessfullyUsedTranslationCount - ToFirstLanguageCount
    /// </summary>
    public int ToFirstLanguageCount { get; set; }
}
