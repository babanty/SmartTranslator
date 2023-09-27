using Microsoft.EntityFrameworkCore;
using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.DataAccess;

public class StatisticsDbContext : DbContext
{
    public DbSet<TextTranslationStatisticsByUserEntity> TextTranslationStatistics { get; set; } = default!;

    public DbSet<HandleMessageFailedStatisticsEntity> HandleMessageFailedStatistics { get; set; } = default!;

    public DbSet<BotSubscriptionStatistics> BotSubscriptionStatistics { get; set; } = default!;

    public StatisticsDbContext(DbContextOptions<StatisticsDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
