using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Db;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class ShowTweetersCommandExtension
{
    public static async Task ShowTweetersCommand(this TelegramClient bot, Message msg, string culture, TelegramDbContext db)
    {

        var chatRegion = await db.ChatRegions
            .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id)
            .Where(x => x.IsActive)
            .Select(x=>x.Region.RegionCode)
            .ToListAsync(bot.Bot.GlobalCancelToken);
        var toShow = await db.Tweeters
            .Where(x => chatRegion
                .Contains(x.Region.RegionCode))
            .Select(t => $"{t.Name} - <a href=\"{t.Link}\">{t.Username}</a>")
            .ToListAsync(bot.Bot.GlobalCancelToken);

        await bot.Bot.SendMessage(msg.Chat,
            text: bot.Rm.GetString(Languages.Command.ShowTweetersContent, CultureInfo.CreateSpecificCulture(culture))! + "\n" + string.Join("\n", toShow),
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            linkPreviewOptions: new LinkPreviewOptions()
            {
                IsDisabled = true,
            } ,
            messageThreadId: msg.MessageThreadId ?? null);
    }
}