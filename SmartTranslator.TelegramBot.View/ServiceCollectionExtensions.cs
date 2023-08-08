using Microsoft.Extensions.DependencyInjection;
using SmartTranslator.TelegramBot.Management.GptTelegramBots;
using SmartTranslator.TelegramBot.View.Filters;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;
using SmartTranslator.TelegramBot.View.Views;
using System.Reflection;

namespace SmartTranslator.TelegramBot.View;

public static class ServiceCollectionExtensions
{
    /// <summary> 
    /// Добавить представление (View в MVC) для телеграм-бота-переводчка.
    /// Для работы так же отдельно необходимо добавить GptTranslationOptions, ITelegramBotMessageSender, TranslationResultProvider, ITemplateStringService, IMapper
    /// </summary>
    /// <param name="assemblies"> AppDomain.CurrentDomain.GetAssemblies() </param>
    public static IServiceCollection AddTelegramTranslatorBotView(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<TelegramBotHandlingStarter>();
        services.AddHostedService<TelegramBotHandlingStarter>();
        services.AddScoped<IGptTelegramBotMessageHandler, TelegramBotMessageHandler>();        
        services.AddScoped<IFiltersHandlerChain, FiltersHandlerChain>();
        services.AddScoped<FilterTools>();

        // filters
        services.AddScoped<ExceptionFilter>();

        services.AddViews(assemblies);

        return services;
    }

    private static IServiceCollection AddViews(this IServiceCollection services, params Assembly[] assemblies)
    {
        var telegramBotViews = assemblies.SelectMany(a => a.GetTypes())
                                         .Where(type => type.GetInterfaces().Contains(typeof(ITelegramBotView)))
                                         .ToList();

        foreach (var telegramBotView in telegramBotViews)
        {
            try
            {
                services.AddScoped(typeof(ITelegramBotView), telegramBotView);
            }
            catch
            {
                // NOTE: заглушка для сервисов, которые необходимо в ручную регистрировать в DI
            }
        }

        return services;
    }
}
