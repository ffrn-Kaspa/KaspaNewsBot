using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Resources.NetStandard;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.TelegramBot.Bot.Extensions;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class HelpCommandExtension
{
    public static async Task<Message> HelpCommand(this TelegramClient bot, Message msg, string culture)
    {
        if (culture != "zh")
            culture = "en";
        
        var message = bot.Rm.GetString(Languages.Command.HelpContet, CultureInfo.CreateSpecificCulture(culture));
        return await bot.Bot.SendMessage(msg.Chat.Id, 
            message!, 
            messageThreadId: msg.MessageThreadId ?? null);
    }
}