using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class SupportCommandExtension
{
    public static async Task SupportCommand(this TelegramClient bot, Message msg, string culture)
    {
        var text = bot.Rm.GetString(Languages.Command.SupportContent, CultureInfo.CreateSpecificCulture(culture))!;
        
        await bot.Bot.SendMessage(msg.Chat, 
            text: text, 
            messageThreadId: msg.MessageThreadId ?? null);
        
        
        await bot.Bot.SendMessage(msg.Chat, 
            text: bot.Rm.GetString(Languages.Command.Address, CultureInfo.CreateSpecificCulture(culture))!, 
            messageThreadId: msg.MessageThreadId ?? null);
    }
}