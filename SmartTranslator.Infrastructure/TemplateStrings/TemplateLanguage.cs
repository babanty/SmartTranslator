namespace SmartTranslator.Infrastructure.TemplateStrings;

/// <summary> Language of speech, items are numbered according to ISO 639-2/B </summary>
public enum TemplateLanguage
{
    Unknown = 0,

    /// <summary> English </summary>
    Eng = 1,

    /// <summary> Russian </summary>
    Rus = 2,

    /// <summary> Turkish </summary>
    Tur = 3,

    /// <summary> Spanish </summary>
    Esp = 4,

    /// <summary> German </summary>
    Deu = 5,

    /// <summary> French </summary>
    Fra = 6,

    /// <summary> Greek </summary>
    Gre = 7,

    /// <summary> Uzbek </summary>
    Uzb = 8,

    /// <summary> Means that text is clear without knowledge about particular language, for example if text consists of only numbers </summary>
    Universal = 1000,

    Rus_Eng = 10000,
    Rus_Tur = 10001,
    Rus_Esp = 10002,
    Rus_Deu = 10003,
    Rus_Fra = 10004,
    Rus_Gre = 10005,
    Rus_Uzb = 10006,
}
