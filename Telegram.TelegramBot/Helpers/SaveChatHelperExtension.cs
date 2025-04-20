
using System.Text;
using Microsoft.EntityFrameworkCore;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Db;
using Telegram.Db.Model;
using Telegram.TelegramBot.Bot;

namespace Telegram.TelegramBot.Helpers;

public static class SaveChatHelperExtension
{
    public static async Task SaveChat(this TelegramClient bot, Message msg,  TelegramDbContext db)
    {
        var chat = await db.Chats.FirstOrDefaultAsync(x => x.ChatIdentificationNumber == msg.Chat.Id,
            bot.Bot.GlobalCancelToken);
        if (chat is not null && !chat.IsDeleted)
        {
            chat.IsDeleted = false;
            db.Chats.Update(chat);
            await db.SaveChangesAsync(bot.Bot.GlobalCancelToken);
            return;
        }

        if (chat is not null)
        {
            chat.IsDeleted = false;
           await db.SaveChangesAsync(bot.Bot.GlobalCancelToken); 
            return;
        }
        ChatDbo toAdd = new ChatDbo
        {
            ChatIdentificationNumber = msg.Chat.Id,
            ChatType = msg.Chat.Type,
            IsDeleted = false,
        };
        if (!await db.Chats.AnyAsync(x => x.ChatIdentificationNumber == msg.Chat.Id, bot.Bot.GlobalCancelToken))
        {
            switch (msg.Chat.Type)
            {
                case ChatType.Private:
                {
                    toAdd.ChatName = msg.Chat.Username ?? new StringBuilder(msg.Chat.FirstName + " " + msg.Chat.LastName).ToString();
                    break;
                }
                case ChatType.Group or ChatType.Channel or ChatType.Supergroup:
                {
                    toAdd.ChatName = msg.Chat.Title!;
                    break;
                }
              
            }
            await db.Chats.AddAsync(toAdd, bot.Bot.GlobalCancelToken);
            await db.SaveChangesAsync(bot.Bot.GlobalCancelToken);
        }

    }
}