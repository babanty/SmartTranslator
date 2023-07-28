namespace SmartTranslator.TranslationCore.Enums;

public enum GptModel
{
    Unknown = 0,

    /// <summary> gpt-4-32k-0613 </summary>
    Gpt4StableLong = 1,

    /// <summary> gpt-4-0613 </summary>
    Gpt4Stable = 2,

    /// <summary> gpt-3.5-turbo-0613 </summary>
    GPT3d5Stable = 3,

    /// <summary> gpt-3.5-turbo-16k-0613 </summary>
    GPT3d5StableLong = 4
}
