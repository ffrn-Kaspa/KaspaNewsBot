using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class SetLanguageCommandExtension
{
    public static async Task<Message> SetLanguageCommand(this TelegramClient bot, Message msg, string culture)
    {
        var startMessage = bot.Rm.GetString(Languages.Command.SetLanguageContent, CultureInfo.CreateSpecificCulture(culture))!;
        
        return await bot.Bot.SendMessage(msg.Chat, startMessage,
            messageThreadId: msg.MessageThreadId ?? null,
            replyMarkup: new InlineKeyboardButton[] { "English", "Chinese" });
    }
}