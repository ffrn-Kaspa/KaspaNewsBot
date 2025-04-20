using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Db;
using Telegram.TelegramBot.Bot;

namespace Telegram.TelegramBot.Helpers;

public static class MovHereSingleHelperExtension
{
    public static async Task MoveHereSingle(this TelegramClient bot, Message msg, string regionName,
        TelegramDbContext db)
    {
        if(msg.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Supergroup)
        {
            return;
        }
        await db.ChatRegions
            .Where(x=> x.Chat.ChatIdentificationNumber == msg.Chat.Id)
            .Where(x=> x.Region.RegionName == regionName)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.TopicId, msg.MessageThreadId), bot.Bot.GlobalCancelToken);
    }
}