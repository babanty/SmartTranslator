namespace SmartTranslator.TelegramBot.View.Filters.Infrastructure;

/// <summary> Contains a simple sequential chain of exception handlers in the required order  </summary>
public class FiltersHandlerChain : IFiltersHandlerChain
{
    private IServiceProvider _serviceProvider;

    public FiltersHandlerChain(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    /// <summary> Process the exception through the existing filters to get a nice message  </summary>
    public Task<string> FilterException(Exception e)
    {
        var filters = GetFilters();

        var filter = filters.FirstOrDefault(f => f.CanHandle(e));

        if (filter is null)
        {
            throw e;
        }

        return filter.Handle(e);
    }


    private Filter[] GetFilters()
    {
        var result = new List<Filter>();

        var children = GetAllDerivedTypes<Filter>();

        foreach (var child in children)
        {
            var filter = _serviceProvider.GetService(child);

            if (filter is Filter castedFilter)
            {
                result.Add(castedFilter);
            }
        }

        return result.OrderByDescending(r => r.Priority)
                     .ToArray();
    }

    private static List<Type> GetAllDerivedTypes<T>()
    {
        var baseType = typeof(T);
        var derivedTypes = new List<Type>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    derivedTypes.Add(type);
                }
            }
        }

        return derivedTypes;
    }
}
