using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telegram.Bot.Types.Enums;


namespace Telegram.Db.Model;

public class ChatDbo : IEntityTypeConfiguration<ChatDbo>
{
    public Guid Id { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public long ChatIdentificationNumber { get; set; } 
    public ChatType ChatType { get; set; }
    public bool IsDeleted { get; set; } = false;

    public ICollection<ChatRegionDbo> Regions { get; set; } = [];
    public ChatLanguageDbo Language { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<ChatDbo> builder)
    {
        builder.ToTable("chats");
        builder.HasKey(c => c.Id);
        builder.HasAlternateKey(c=>c.ChatIdentificationNumber);
        builder.Property(c => c.ChatName).IsRequired();
        builder.Property(c => c.ChatIdentificationNumber).IsRequired();
        builder.Property(c => c.ChatType).IsRequired();
        
    }
}