using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static  class AddTweeterCommandExtension
{

    public static async Task AddTweeterCommand(this TelegramClient bot, Message msg, string culture)
    {
        
    await bot.Bot.SendMessage(msg.Chat, 
        text: bot.Rm.GetString(Languages.Command.AddTweeterContent, CultureInfo.CreateSpecificCulture(culture))!, 
        messageThreadId: msg.MessageThreadId ?? null);
    }
}