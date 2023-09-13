using Microsoft.Extensions.DependencyInjection;

namespace SmartTranslator.Infrastructure.UserLanguages;

public static class ServiceCollectionExtension
{
    /// <summary> Adds IUserLanguageProvider to DI </summary>
    public static IServiceCollection AddUserLanguageProvider(this IServiceCollection services)
    {
        services.AddScoped<IUserLanguageProvider, UserLanguageProviderStub>();

        return services;
    }
}
