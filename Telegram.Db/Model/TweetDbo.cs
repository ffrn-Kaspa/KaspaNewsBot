using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telegram.Db.Model;

public class TweetDbo : IEntityTypeConfiguration<TweetDbo>
{
    public Guid Id { get; set; }
    public Guid TweeterId { get; set; } 
    public string Tweet { get; set; } = string.Empty;
    public DateTime TweetDate { get; set; }
    
    public TweeterDbo Tweeter { get; set; } = null!;

    public void Configure(EntityTypeBuilder<TweetDbo> builder)
    {
        builder.ToTable("tweets");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Tweet).IsRequired();
        builder.Property(c=>c.TweetDate)
            .HasDefaultValueSql("NOW()").IsRequired();
        builder.HasOne(c => c.Tweeter)
            .WithMany(c => c.Tweets)
            .HasForeignKey(c => c.TweeterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}