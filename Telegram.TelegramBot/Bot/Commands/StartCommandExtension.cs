using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.TelegramBot.Helpers;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Bot.Commands;

public static class StartCommandExtension 
{
    public static async Task<Message> StartCommand(this TelegramClient bot, Message msg, string culture)
    {
        //save chat if not saved
        
        var startMessage = bot.Rm.GetString(Languages.Command.StartContent, CultureInfo.CreateSpecificCulture(culture))!;
        
        return await bot.Bot.SendMessage(
            chatId: msg.Chat, 
            text: startMessage,
            messageThreadId: msg.MessageThreadId ?? null,
            replyMarkup: new InlineKeyboardButton[] { "Help", "Set Language" });
    }
}