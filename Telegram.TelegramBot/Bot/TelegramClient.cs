using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Db;
using Telegram.TelegramBot.Bot.Commands;
using Telegram.TelegramBot.Helpers;
using Telegram.TelegramBot.Localization.Tweet;
using Telegram.TelegramBot.Settings;


namespace Telegram.TelegramBot.Bot;

public class TelegramClient 
{
    private readonly ILogger<TelegramClient> _logger;
    private readonly TelegramSettings _settings;
    private readonly HttpClient _client;
    private readonly IServiceProvider _serviceProvider;
    protected internal TelegramBotClient Bot { get; set; } = null!;
    protected internal ResourceManager Rm { get; }
    private User _me = null!;
    private CancellationTokenSource _tokenSource;
    
    public TelegramClient(ILogger<TelegramClient> logger, IOptions<TelegramSettings> settings, HttpClient client, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _client = client;
        _serviceProvider = serviceProvider;
      
        _settings = settings.Value;
        _tokenSource = new CancellationTokenSource(); 
        Rm = new ResourceManager($"Telegram.TelegramBot.Localization.Tweet.Resources", Assembly.GetExecutingAssembly());
    }




    protected internal async Task Setup()
    {
        Bot = new TelegramBotClient(
            token: _settings.Token,
            httpClient: _client,
            cancellationToken: _tokenSource.Token);
        _me = await Bot.GetMe();
        Bot.OnError += OnError;
        Bot.OnMessage += OnMessage;
        Bot.OnUpdate += OnUpdate;
    }
    
    async Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
    }


    async Task OnMessage(Message msg, UpdateType type)
    {
      
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TelegramDbContext>();
        var chatLang = await db.ChatLanguages
            .Where(x => x.Chat.ChatIdentificationNumber == msg.Chat.Id)
            .Select(x=>x.Region.RegionCode)
            .FirstOrDefaultAsync(Bot.GlobalCancelToken);        
        var culture = "en";
        if (chatLang is not null)
        {
            culture = chatLang;
        }
        
        
        switch (msg.Text is not null? msg.Text.Split('@')[0] : "")
        {
            case "/start":
                if(!await Protect(msg, culture)){return;}
                
                await this.SaveChat(msg, db);
                await this.StartCommand(msg, culture);
                return;
            case "/help":
                
                await this.HelpCommand(msg, culture);
                return;
            case "/language":
                if(!await Protect(msg, culture)){return;}
                
                await this.SetLanguageCommand(msg, culture);
                return;
            case "/enable":
                if(!await Protect(msg, culture)){return;}
                
                await this.EnableCommand(msg, culture, db);
                break;
            case "/disable":
                if(!await Protect(msg, culture)){return;}
                
                await this.DisableCommand(msg, culture, db);
                break;
            case "/getenabled" :
                await this.GetEnabledRegions(msg, culture, db);
                break;
            case "/movehere":
                if(!await Protect(msg, culture)){return;}
                
                await this.MoveHereAllCommand(msg, culture, db);
                break;
            case "/moveheresingle":
                if(!await Protect(msg, culture)){return;}
                
                await this.MoveHereSingleCommand(msg, culture, db);
                break;
            case "/showtwitters":
                if(!await Protect(msg, culture)){return;}
                
                await this.ShowTweetersCommand(msg, culture, db);
                break;
            case "/addtwitter":
                await this.AddTweeterCommand(msg, culture);
                break;
            case "/support":
                await this.SupportCommand(msg, culture);
                break;
        } 
        
    }

// method that handle other types of updates received by the bot:
    async Task OnUpdate(Update update)
    {
        
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TelegramDbContext>();

        if (update is { MyChatMember: { } newStatus })
        {
            if (newStatus.NewChatMember.Status == ChatMemberStatus.Kicked)
            {
                await db.Chats.Where(x => x.ChatIdentificationNumber == newStatus.Chat.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsDeleted, true), Bot.GlobalCancelToken);

            }
        }
        if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
        {
            
            var setLanguageSuccess = Languages.Success.SetLanguageSuccess;
            var enableContentSuccess = Languages.Success.EnableContentSuccess;
            var disableContentSuccess = Languages.Success.DisableContentSuccess;
            var moveHereSingleSuccess = Languages.Success.MoveHereSingleSuccess;
            
            var chatLang = await db.ChatLanguages
                .Where(x => x.Chat.ChatIdentificationNumber == update.CallbackQuery.Message.Chat.Id)
                .Select(x=>x.Region.RegionCode)
                .FirstOrDefaultAsync(Bot.GlobalCancelToken);
            var culture = "en";
            if (chatLang is not null)
            {
                culture = chatLang;
            }
            if(!await Protect(query.Message!, culture, query)){return;}
            switch (query.Data)
            {
                //region from start command
                case "Help":
                    if (query.Message != null)
                    {
                        await this.HelpCommand(query.Message, culture);
                    }
                    break;
                case "Set Language":
                    if (query.Message != null)
                    {
                        await this.SetLanguageCommand(query.Message, culture);
                    }
                    break;
                //region from set language command
                case "English":
                    if (query.Message != null)
                    {
                        await this.SetChatLanguage(query.Message.Chat.Id, "en", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(setLanguageSuccess, CultureInfo.CreateSpecificCulture("en"))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Chinese":
                    if (query.Message != null)
                    {
                        await this.SetChatLanguage(query.Message.Chat.Id, "zh", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(setLanguageSuccess, CultureInfo.CreateSpecificCulture("zh"))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                //region from register twee languages or remove languages
                case "China":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "China", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Global":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Global", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Italy":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Italy", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Russia":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Russia", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Germany":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Germany", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Spain":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Spain", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Portugal":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Portugal", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Japanese":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Japanese", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Hebrew":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "Hebrew", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "France":
                    if (query.Message != null)
                    {
                        await this.RegisterTweetRegion(query.Message.Chat.Id, "France", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(enableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Remove China":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "China", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Remove Global":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Global", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Remove Italy":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Italy", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove Russia":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Russia", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove Germany":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Germany", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove Spain":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Spain", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove Portugal":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Portugal", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove Japanese":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Japanese", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove Hebrew":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "Hebrew", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Remove France":
                    if (query.Message != null)
                    {
                        await this.RemoveTweetRegion(query.Message.Chat.Id, "France", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(disableContentSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Move Global":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Global", db);
                        await Bot.SendMessage(query.Message.Chat.Id,                        
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }
                    break;
                case "Move China":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "China", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Italy":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Italy", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Russia":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Russia", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Germany":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Germany", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Spain":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Spain", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Portugal":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Portugal", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Japanese":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Japanese", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move Hebrew":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "Hebrew", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                case "Move France":
                    if (query.Message != null)
                    {
                        await this.MoveHereSingle(query.Message, "France", db);
                        await Bot.SendMessage(query.Message.Chat.Id,
                            Rm.GetString(moveHereSingleSuccess, CultureInfo.CreateSpecificCulture(culture))!,
                            messageThreadId: query.Message.MessageThreadId ?? null);
                    }

                    break;
                default:
                    await Bot.AnswerCallbackQuery(query.Id, "Unknown command");
                    break;
            }
            
        }
    }

    

    protected internal async Task SendMessage(string content, string chatId)
    {
        await Bot.SendMessage(
            chatId: chatId,
            text: content,
            parseMode: ParseMode.Html);
    }

    private async Task<bool> Protect(Message msg, string culture, CallbackQuery? callback = null)
    {
        var member = await Bot.GetChatMember(msg.Chat.Id, msg.From!.Id);
        if (callback is not null)
        {
            member = await Bot.GetChatMember(callback.Message!.Chat.Id, callback.From.Id);
        }
        if (msg.Chat.Type is ChatType.Private
            || member.Status == ChatMemberStatus.Administrator
            || member.Status == ChatMemberStatus.Creator)
        {
            return true;
        }
        await Bot.SendMessage(msg.Chat.Id,
            Rm.GetString(Languages.Authorization.NotAdmin, CultureInfo.CreateSpecificCulture(culture))!,
            messageThreadId: msg.MessageThreadId ?? null);
        return false;
    }
}