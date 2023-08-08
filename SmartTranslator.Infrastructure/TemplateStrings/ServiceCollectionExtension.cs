using Microsoft.Extensions.DependencyInjection;

namespace SmartTranslator.Infrastructure.TemplateStrings;

public static class ServiceCollectionExtension
{
    /// <summary> Добавляет в DI ITemplateStringService </summary>
    public static IServiceCollection AddTemplateStringService(this IServiceCollection services)
    {
        services.AddScoped<TemplateStringDbContext>();
        services.AddScoped<ITemplateStringService, TemplateStringService>();

        return services;
    }
}
