using Telegram.Db.Enums;

namespace Telegram.DiscordBot.Model;

public class MessageDto
{
    public required AllowedRegion Region { get; set; }
    public required string? Quoted { get; set; }
    public required string? Replying { get; set; }
    public required string? Retweeted { get; set; }
    public required string User { get; set; } = string.Empty;
    public required string Username { get; set; } = string.Empty;
    public required string Message { get; set; } = string.Empty;
    public required string Source { get; set; } = string.Empty;
    public required bool IsReply { get; set; } 
    
}