using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Db;
using Telegram.Db.Model;
using Telegram.TelegramBot.Bot;

namespace Telegram.TelegramBot.Helpers;

public static class SetChatLanguageHelperExtension
{
    public static async Task SetChatLanguage(this TelegramClient bot, long chatId, string languageCode, TelegramDbContext db)
    {
        var regionId = await db.AllowedLanguages
            .Where(x => x.RegionCode == languageCode)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(bot.Bot.GlobalCancelToken);
        if (await db.ChatLanguages.AnyAsync(x => x.ChatId == chatId, bot.Bot.GlobalCancelToken))
        {
            await db.ChatLanguages
                .Where(x => x.ChatId == chatId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.RegionId, regionId),bot.Bot.GlobalCancelToken);
            
            return;
        }
        await db.ChatLanguages
            .AddAsync(new ChatLanguageDbo
            {
                ChatId = chatId,
                RegionId = regionId
            },bot.Bot.GlobalCancelToken);
        await db.SaveChangesAsync(bot.Bot.GlobalCancelToken);
       
    }

}