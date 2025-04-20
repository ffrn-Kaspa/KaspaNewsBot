using Microsoft.EntityFrameworkCore;
using Telegram.Db.Model;

namespace Telegram.Db;

public class TelegramDbContext :DbContext
{
    public DbSet<ChatDbo> Chats { get; set; } = null!;
    public DbSet<ChatLanguageDbo> ChatLanguages { get; set; } = null!;
    public DbSet<ChatRegionDbo> ChatRegions { get; set; } = null!;
    public DbSet<TweetDbo> Tweets { get; set; } = null!;
    public DbSet<TweeterDbo> Tweeters { get; set; } = null!;
    public DbSet<RegionDbo> AllowedLanguages { get; set; } = null!;
    
    public TelegramDbContext(DbContextOptions<TelegramDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelegramDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder optionsBuilder)
    {
        base.ConfigureConventions(optionsBuilder);
        optionsBuilder.Properties<Enum>().HaveConversion<string>();
    }
}