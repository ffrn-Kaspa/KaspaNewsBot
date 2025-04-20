using System.ComponentModel.DataAnnotations;

namespace Telegram.DiscordBot.Settings;

public class DiscordSettings
{
    public const string Section = "Discord";
    [Required]
    public string Token { get; set; } = string.Empty;
    public string? TestToken { get; set; } = string.Empty;
    [Required]
    public string PostUrl { get; set; } = string.Empty;
}