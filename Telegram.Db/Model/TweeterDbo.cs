using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telegram.Db.Model;

public class TweeterDbo : IEntityTypeConfiguration<TweeterDbo>
{
    public Guid Id { get; set; }
    public Guid RegionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    
    public RegionDbo Region { get; set; } = null!;
    
    public List<TweetDbo> Tweets { get; set; } = [];
    public void Configure(EntityTypeBuilder<TweeterDbo> builder)
    {
        builder.ToTable("tweeters");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name)
            .HasMaxLength(128)
            .IsRequired();
        builder.Property(c => c.Link)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(c => c.Username)
            .HasMaxLength(64)
            .IsRequired();
        builder.HasOne(c => c.Region)
            .WithMany(c => c.Tweeters)
            .HasForeignKey(c => c.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}