namespace SmartTranslator.TelegramBot.View.Filters.Infrastructure;

public interface IFiltersHandlerChain
{
    Task<string> FilterException(Exception e);
}