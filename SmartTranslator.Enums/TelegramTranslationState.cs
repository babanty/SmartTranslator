namespace SmartTranslator.Enums;

public enum TelegramTranslationState
{
    Unknown = 0,
    Created = 1,
    WaitingForLanguage = 2,
    WaitingForContext = 3,
    WaitingForStyle = 4,
    WaitingForTranslation = 5,
    Finished = 6
}
