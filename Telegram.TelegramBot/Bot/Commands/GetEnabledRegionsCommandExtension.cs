using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Db;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class GetEnabledRegionsCommandExtension
{
    public static async Task GetEnabledRegions(this TelegramClient bot, Message msg, string culture, TelegramDbContext db)
    {
        var regions = await db.Chats
            .Where(x => x.ChatIdentificationNumber == msg.Chat.Id)
            .SelectMany(x => x.Regions
                .Where(y=>y.IsActive)
                .Select(y=>y.Region.RegionName))
            .ToListAsync(bot.Bot.GlobalCancelToken);
        
        var formatted = string.Join("\n", regions
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / 3)
            .Select(g => string.Join(" | ", g.Select(x => x.item)))
        );
        var startMessage = bot.Rm.GetString(Languages.Command.GetEnabledRegionContent, CultureInfo.CreateSpecificCulture(culture))!;
        var sb = new StringBuilder(startMessage)
            .Append("\n")
            .Append(formatted);
        await bot.Bot.SendMessage(msg.Chat, 
            sb.ToString(),
            messageThreadId: msg.MessageThreadId ?? null);
    }
}