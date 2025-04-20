using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Db;
using Telegram.Db.Enums;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class DisableCommandExtension
{
    
    
    public static async Task DisableCommand(this TelegramClient bot, Message msg, string culture, TelegramDbContext db)
    {
        var startMessage = bot.Rm.GetString(Languages.Command.DisableContent, CultureInfo.CreateSpecificCulture(culture))!;

        var regions = db.ChatRegions
            .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id)
            .Where(x=>x.IsActive)
            .Select(x => x.Region.RegionName)
            .ToList();
        var buttons = regions.Select(region =>
            InlineKeyboardButton.WithCallbackData(
                text: "Remove "+region,         
                callbackData: "Remove "+region  
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