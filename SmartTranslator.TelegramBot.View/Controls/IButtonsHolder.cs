using System.Reflection;

namespace SmartTranslator.TelegramBot.View.Controls;

public interface IButtonsHolder
{
    IReadOnlyCollection<string> Buttons => GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                                    .Select(i => i.GetValue(this)!.ToString()!)
                                                    .ToList();
}