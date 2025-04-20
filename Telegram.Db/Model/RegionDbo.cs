using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telegram.Db.Model;

public class RegionDbo :IEntityTypeConfiguration<RegionDbo>
{
    public Guid Id { get; set; }
    public string RegionCode { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;

    public List<ChatRegionDbo> Chats { get; set; } = [];
    public List<TweeterDbo> Tweeters { get; set; } = [];
    public List<ChatLanguageDbo> ChatLanguages { get; set; } = [];

    public void Configure(EntityTypeBuilder<RegionDbo> builder)
    {
        builder.ToTable("regions");
        builder.HasKey(c => c.Id);
        builder.HasAlternateKey(c => c.RegionCode);
        builder.HasAlternateKey(c => c.RegionName);
        builder.Property(c => c.RegionCode).IsRequired();
        builder.Property(c => c.RegionName).IsRequired();

        
    }
}
