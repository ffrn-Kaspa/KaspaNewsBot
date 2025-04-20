using Telegram.Bot.Types;
using Telegram.Db.Enums;

namespace Telegram.TelegramBot.Bot.Extensions;

public static class CultureIdentifierMethodExtension
{

    public static string RegionMatcher(this TelegramClient bot, string? langCode)
    {
        if (string.IsNullOrEmpty(langCode))
            return "en";
        switch (langCode)
        {
            case "zh":
                return "zh";
            case "cn":
                return "zh";
            case "it":
                return "it";
            case "ru":
                return "ru";
            case "de":
                return "de";
            case "es":
                return "es";
            case "pt":
                return "pt";
            case "jp":
                return "jp";
            case "he":
                return "he";
            default:
                return "he";
        }
    }
}