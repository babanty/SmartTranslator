using Microsoft.Extensions.DependencyInjection;
using SmartTranslator.TranslationCore.Abstractions;

namespace SmartTranslator.TranslationCore.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTranslationCore(this IServiceCollection services, TranslationCoreOptions config)
    {
        services.AddSingleton(config.GptHttpClientOptions);
        services.AddSingleton(config.GptTranslationOptions);
        services.AddSingleton(config.LanguageOptions);        

        services.AddScoped<IGptHttpClient, GptHttpClient>();
        services.AddScoped<IGptTranslator, GptTranslator>();
        services.AddScoped<ILanguageManager, LanguageManager>();
        services.AddScoped<ITextMistakeManager, TextMistakeManager>();

        return services;
    }
}
