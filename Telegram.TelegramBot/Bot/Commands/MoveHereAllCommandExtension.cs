using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Db;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class MoveHereAllCommandExtension
{
    public static async Task MoveHereAllCommand(this TelegramClient bot, Message msg, string culture, TelegramDbContext db)
    {
        string moveHereSuccess = Languages.Success.MoveHereSuccess;
        if (msg.Chat.Type != ChatType.Supergroup)
        {
            return;
        }
        int? topic = msg.MessageThreadId!;
        if (topic is not null)
        {
            await db.ChatRegions
                .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.TopicId, topic), bot.Bot.GlobalCancelToken);
        }
        else
        {
            var chats = await db.ChatRegions
                .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id).ToListAsync(bot.Bot.GlobalCancelToken);
            chats.ForEach(x =>
            {
                x.TopicId = null;
            });
            await db.SaveChangesAsync(bot.Bot.GlobalCancelToken);
        }
        await bot.Bot.SendMessage(msg.Chat.Id,
            bot.Rm.GetString(moveHereSuccess, CultureInfo.CreateSpecificCulture(culture))!,
            messageThreadId: msg.MessageThreadId ?? null);
    }
}