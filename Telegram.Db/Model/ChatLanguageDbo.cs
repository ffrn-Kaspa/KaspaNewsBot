using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telegram.Db.Model;

public class ChatLanguageDbo : IEntityTypeConfiguration<ChatLanguageDbo>
{
    public Guid Id { get; set; }
    public long ChatId { get; set; }
    public Guid RegionId { get; set; } 

    public ChatDbo Chat { get; set; } = null!;
    public RegionDbo Region { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<ChatLanguageDbo> builder)
    {
        builder.ToTable("chat_language");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Chat)
            .WithOne(x => x.Language)
            .HasForeignKey<ChatLanguageDbo>(x => x.ChatId)
            .HasPrincipalKey<ChatDbo>(x => x.ChatIdentificationNumber)     
            .OnDelete(DeleteBehavior.Cascade);
        
       builder.HasOne(x => x.Region)
            .WithMany(x => x.ChatLanguages)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
