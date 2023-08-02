namespace SmartTranslator.Infrastructure.Extensions;

public static class IEnumerableExtensions
{
    public static T? Second<T>(this IEnumerable<T> values)
    {
        return values.ElementAtOrDefault(1);
    }

    public static T? Second<T>(this T[] values)
    {
        return values.Length >= 2 ? values[1] : default;
    }
}
