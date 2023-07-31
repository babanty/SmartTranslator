namespace SmartTranslator.TelegramBot.View.Filters.Infrastructure;

public abstract class Filter
{
    public abstract Task<string> Handle(Exception e);

    public abstract bool CanHandle(Exception e);

    public int Priority { get; protected set; } = 0;
}
