using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmartTranslator.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    /// <summary> Добавить настройки из json в DI </summary>
    /// <returns> Возвращает заполненный из json конфиги </returns>
    /// <param name="services"/>
    /// <param name="configuration"/>
    /// <param name="key"> секция в Json </param>
    /// <typeparam name="T"> Тип класса с конфигами </typeparam>
    public static T AddConfig<T>(this IServiceCollection services, IConfiguration configuration, string key = "")
            where T : class, new()
    {
        var config = string.IsNullOrEmpty(key) ? configuration.Get<T>() : configuration.GetSection(key).Get<T>();
        services.AddSingleton(config);

        return config;
    }
}
