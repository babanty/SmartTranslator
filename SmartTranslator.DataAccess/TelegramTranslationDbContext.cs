using Microsoft.EntityFrameworkCore;
using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.DataAccess;

public class TelegramTranslationDbContext : DbContext
{
    public DbSet<TelegramTranslationEntity> TelegramTranslations { get; set; } = default!;

    public TelegramTranslationDbContext() => Database.EnsureCreated();

    public TelegramTranslationDbContext(DbContextOptions<TelegramTranslationDbContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramTranslationEntity>().OwnsMany(p => p.Contexts);
    }
}
