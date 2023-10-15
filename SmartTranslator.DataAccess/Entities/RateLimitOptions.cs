namespace SmartTranslator.DataAccess.Entities;

public record RateLimitOptions
{
    public RateLimit[] RateLimits { get; set; } = default!;
}
