using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telegram.Db.Model;

public class ChatRegionDbo :IEntityTypeConfiguration<ChatRegionDbo>
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid RegionId { get; set; }
    public int? TopicId { get; set; }
    public bool IsActive { get; set; }
    
    public ChatDbo Chat { get; set; } = null!;
    public RegionDbo Region { get; set; } = null!;

    public void Configure(EntityTypeBuilder<ChatRegionDbo> builder)
    {
        builder.ToTable("chat_regions");
        builder.HasKey(c => c.Id);
        builder.HasOne(c => c.Chat)
            .WithMany(c => c.Regions)
            .HasForeignKey(c => c.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.Region)
            .WithMany(c => c.Chats)
            .HasForeignKey(c => c.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
    }
}
