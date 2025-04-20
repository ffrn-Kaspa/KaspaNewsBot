using System.ComponentModel.DataAnnotations;

namespace Telegram.TelegramBot.Settings;

public class TelegramSettings
{
    public const string Section = "Telegram";
    [Required]
    public string Token { get; set; } = string.Empty;
    public string? TestToken { get; set; } = string.Empty;

}