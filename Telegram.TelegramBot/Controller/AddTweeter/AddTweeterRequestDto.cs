using System.ComponentModel.DataAnnotations;
using Telegram.TelegramBot.Controller.NewTweet;

namespace Telegram.TelegramBot.Controller.AddTweeter;

public class AddTweeterRequestDto
{
  
    [Required]
    public string TweeterName { get; set; } = string.Empty;
    [Required]
    public string RegionCode { get; set; } = string.Empty;
    [Required]
    public string TweeterLink { get; set; } = string.Empty;
    [Required]
    public string TweeterUsername { get; set; } = string.Empty;
}