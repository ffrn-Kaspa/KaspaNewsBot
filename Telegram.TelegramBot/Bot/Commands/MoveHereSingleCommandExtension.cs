using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Db;
using Telegram.Db.Enums;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class MoveHereSingleCommandExtension
{
    public static async Task MoveHereSingleCommand(this TelegramClient bot, Message msg, string culture, TelegramDbContext db)
    {
        if (msg.Chat.Type != ChatType.Supergroup)
        {
            return;
        }
        var startMessage = bot.Rm.GetString(Languages.Command.MoveHereSingle, CultureInfo.CreateSpecificCulture(culture))!;
        var regions = db.ChatRegions
            .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id)
            .Where(x=>x.IsActive)
            .Select(x => x.Region.RegionName)
            .ToList();
        var buttons = regions.Select(region =>
            InlineKeyboardButton.WithCallbackData(
                text: "Move "+region,         
                callbackData: "Move "+region  
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