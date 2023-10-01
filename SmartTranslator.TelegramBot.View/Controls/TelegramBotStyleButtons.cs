using System.Reflection;

namespace SmartTranslator.TelegramBot.View.Controls;

public class TelegramBotStyleButtons : IButtonsHolder
{
    /// <summary> Translate in scientific style </summary>
    public const string ScientificStyle = @"📖 scientific style";

    /// <summary> Translate in official style </summary>
    public const string OfficialStyle = @"👔 official style";

    /// <summary> Translate in teenage style </summary>
    public const string TeenageStyle = @"🛹 teenage style";

    /// <summary> Translate in teenage style </summary>
    public const string ConversationalStyle = @"💬 conversational style";

    public IReadOnlyCollection<string> Buttons =>
    GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
             .Select(i => i.GetValue(this)?.ToString() ?? "")
             .ToList();
}
