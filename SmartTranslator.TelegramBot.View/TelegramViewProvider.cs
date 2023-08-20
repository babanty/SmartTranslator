using SmartTranslator.TelegramBot.View.Views;

namespace SmartTranslator.TelegramBot.View;

public class TelegramViewProvider
{
    private readonly List<ITelegramBotView> _telegramBotViews;

    public TelegramViewProvider(List<ITelegramBotView> telegramBotViews)
    {
        _telegramBotViews = telegramBotViews;
    }


    public T GetView<T>() where T : ITelegramBotView
    {
        var view = _telegramBotViews.OfType<T>().FirstOrDefault();

        return view is null ? throw new InvalidOperationException($"No handler found for a message of type {typeof(T).Name}") : view;
    }
}
