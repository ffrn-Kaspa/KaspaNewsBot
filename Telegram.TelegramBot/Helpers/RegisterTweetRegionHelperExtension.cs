using Microsoft.EntityFrameworkCore;
using Telegram.Db;
using Telegram.Db.Model;
using Telegram.TelegramBot.Bot;

namespace Telegram.TelegramBot.Helpers;

public static class RegisterTweetRegionHelperExtension
{
    public static async Task RegisterTweetRegion(this TelegramClient bot, long chatId, string regionName, TelegramDbContext db)
    {
        var regionExists = await db.ChatRegions
            .Include(cr => cr.Region)
            .FirstOrDefaultAsync(cr =>
                    cr.Chat.ChatIdentificationNumber == chatId &&
                    cr.Region.RegionName == regionName,
                bot.Bot.GlobalCancelToken);

        if (regionExists is not null && !regionExists.IsActive)
        {
            await db.ChatRegions
                .Where(x => x.ChatId == regionExists.ChatId && x.RegionId == regionExists.RegionId)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsActive, true), bot.Bot.GlobalCancelToken);
            return;
        }
        
        if (regionExists is not null)
        {
            return;
        }

        var regionId = await db.AllowedLanguages
            .Where(x => x.RegionName == regionName)
            .Select(x => x.Id)
            .FirstAsync(bot.Bot.GlobalCancelToken);

        var chatIdDb = await db.Chats
            .Where(x => x.ChatIdentificationNumber == chatId)
            .Select(x => x.Id)
            .FirstAsync(bot.Bot.GlobalCancelToken);

        var newRegion = new ChatRegionDbo
        {
            ChatId = chatIdDb,
            RegionId = regionId,
            IsActive = true
        };

        await db.ChatRegions.AddAsync(newRegion, bot.Bot.GlobalCancelToken);
        await db.SaveChangesAsync();
    }
}