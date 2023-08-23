using Microsoft.Extensions.DependencyInjection;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.View.Exceptions;
using SmartTranslator.TelegramBot.View.Views;

namespace SmartTranslator.TelegramBot.View;

public class TelegramViewProvider
{
    private readonly IServiceProvider _serviceProvider;

    public TelegramViewProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public T GetView<T>() where T : ITelegramBotView
    {
        var telegramBotViews = _serviceProvider.GetServices<ITelegramBotView>().ToList();
        var view = telegramBotViews.OfType<T>().FirstOrDefault();

        return view is null ? throw new InvalidOperationException($"No handler found for a message of type {typeof(T).Name}") : view;
    }
}
