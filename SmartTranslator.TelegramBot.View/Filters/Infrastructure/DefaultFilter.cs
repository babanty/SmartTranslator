namespace SmartTranslator.TelegramBot.View.Filters.Infrastructure;

public abstract class DefaultFilter<T> : Filter
    where T : Exception
{
    public abstract Task<string> Handle(T e);



    public override bool CanHandle(Exception e) => e.GetType().FullName == typeof(T).FullName;


    public override Task<string> Handle(Exception e)
    {
        if (!CanHandle(e))
        {
            throw new ArgumentException($"Filter that handles {typeof(T).Name} can't handle {e.GetType().Name}.");
        }

        return Handle(e as T ?? throw new Exception("Have already checked that higher."));
    }
}
