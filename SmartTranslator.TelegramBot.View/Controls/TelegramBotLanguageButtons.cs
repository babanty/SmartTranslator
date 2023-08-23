using System.Reflection;

namespace SmartTranslator.TelegramBot.View.Controls;

public class TelegramBotLanguageButtons : IButtonsHolder
{
    /// <summary> English language </summary>
    public const string English = "🇬🇧 English";

    /// <summary> Russian language </summary>
    public const string Russian = "🇷🇺 Russian";

    public IReadOnlyCollection<string> Buttons =>
    GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
             .Select(i => i.GetValue(this)?.ToString() ?? "")
             .ToList();
}
