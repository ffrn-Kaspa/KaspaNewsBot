using Microsoft.EntityFrameworkCore;
using Telegram.Db;
using Telegram.Db.Model;
using Telegram.TelegramBot.Bot;

namespace Telegram.TelegramBot.Helpers;

public static class RemoveTweetRegionHelperExtension
{
    public static async Task RemoveTweetRegion(this TelegramClient bot, long chatId, string regionName, TelegramDbContext db)
    {
        var regionExists = await db.ChatRegions
            .Include(cr => cr.Region)
            .FirstOrDefaultAsync(cr =>
                    cr.Chat.ChatIdentificationNumber == chatId &&
                    cr.Region.RegionName == regionName,
                bot.Bot.GlobalCancelToken);

        if (regionExists is  null || !regionExists.IsActive)
        {
            return;
        }
        await db.ChatRegions
            .Where(x => x.ChatId == regionExists.ChatId && x.RegionId == regionExists.RegionId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsActive, false), bot.Bot.GlobalCancelToken);
        
       
    }
}