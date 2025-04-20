using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Db;
using Telegram.Db.Enums;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class EnableCommandExtension
{
    public static AllowedRegion[] allowedRegions = new[]
    {
        AllowedRegion.China,
        AllowedRegion.Global,
        AllowedRegion.Italy,
        AllowedRegion.Russia,
        AllowedRegion.Germany,
        AllowedRegion.Spain,
        AllowedRegion.Portugal,
        AllowedRegion.Japanese,
        AllowedRegion.Hebrew,
        AllowedRegion.France
    };
    public static async Task EnableCommand(this TelegramClient bot, Message msg, string culture, TelegramDbContext db)
    {
        
        
        var startMessage = bot.Rm.GetString(Languages.Command.EnableContent, CultureInfo.CreateSpecificCulture(culture))!;
        
        var chats = await db.ChatRegions
            .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id)
            .Where(x => x.IsActive)
            .Select(x => x.Region.RegionName)
            .ToListAsync(bot.Bot.GlobalCancelToken);
        
        var notEnabled = allowedRegions
            .Where(x => !chats.Contains(x.ToString()))
            .ToList();
        var buttons = notEnabled.Select(region =>
            InlineKeyboardButton.WithCallbackData(
                text: region.ToString(),         
                callbackData: region.ToString()  
            )).ToList();

        var keyboard = new InlineKeyboardMarkup(
            buttons
                .Select((button, index) => new { button, index })
                .GroupBy(x => x.index / 2)
                .Select(g => g.Select(x => x.button).ToArray())
        );

        await bot.Bot.SendMessage(msg.Chat, startMessage,
            messageThreadId: msg.MessageThreadId ?? null,
            replyMarkup: keyboard);
    }
}