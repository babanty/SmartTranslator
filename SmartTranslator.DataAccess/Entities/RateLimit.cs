namespace SmartTranslator.DataAccess.Entities;

public record RateLimit
{
    public int AllowedTranslations { get; set; }
    public int TimeSpanInSeconds { get; set; }
}
