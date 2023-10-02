using System.Reflection;

namespace SmartTranslator.TelegramBot.View.Controls;

public class TelegramBotInlineButtons : IButtonsHolder
{
    public const string Like = "👍";
    public const string Dislike = "👎";

    public IReadOnlyCollection<string> Buttons =>
    GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
         .Select(i => i.GetValue(this)?.ToString() ?? "")
         .ToList();
}
