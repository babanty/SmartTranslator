using System.Reflection;

namespace SmartTranslator.TelegramBot.View.Controls;

public class TelegramBotButtons : IButtonsHolder
{
    /// <summary> The bot has been turned on </summary>
    public const string Start = @"/start";

    /// <summary> Regularly translate text </summary>
    public const string Translate = "Translate new text accurately / Качественно перевести новый текст";

    public IReadOnlyCollection<string> Buttons =>
    GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
             .Select(i => i.GetValue(this)?.ToString() ?? "")
             .ToList();
}
