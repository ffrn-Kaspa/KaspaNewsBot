using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Db;
using Telegram.Db.Enums;
using Telegram.TelegramBot.Bot;
using Telegram.TelegramBot.Localization.Tweet;

namespace Telegram.TelegramBot.Controller.NewTweet;
[ApiController]
[Route("new")]
public class NewTweetController : ControllerBase
{
    private readonly ILogger<NewTweetController> _logger;
    private readonly TelegramClient _client;
    private readonly TelegramDbContext _db;
    private string _culture;

    public NewTweetController(ILogger<NewTweetController> logger, TelegramClient client, TelegramDbContext db)
    {
        _logger = logger;
        _client = client;
        _db = db;
        _culture = "en";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] NewTweetRequestDto request, CancellationToken cancellationToken)
    {
        _culture = RegionConverter(request.Region);
        if (_culture != "zh")
        {
            _culture = "en";
        }
        string header = GetString(Languages.Tweet.NewTweet);
        
        var sb = new StringBuilder(header);
        sb.Append($"<a href=\"{request.User}\">{request.Username}</a>");
        sb.Append("\n");
        sb.Append("\n");
        sb.Append(request.Message);
        sb.Append("\n\n");
        if (request.IsReply)
        {
            if (request.Quoted is not null)
            {
                sb.Append(GetString(Languages.Tweet.Quoted));
                sb.Append(" ");
                sb.Append($"<a href=\"{request.Source}\">{GetString(Languages.Tweet.Source)}</a>");
                sb.Append("\n\n");
                sb.Append(GetString(Languages.Tweet.SourceText));
                sb.Append(" ");
                sb.Append($"<a href=\"{request.Quoted}\">{GetString(Languages.Tweet.Original)}</a>");
            }
            else if (request.Retweeted is not null)
            {
                sb.Append(GetString(Languages.Tweet.Retweet));
                sb.Append(" ");
                
                sb.Append($"<a href=\"{request.Source}\">{GetString(Languages.Tweet.Source)}</a>");
                sb.Append("\n\n");
                sb.Append(GetString(Languages.Tweet.SourceText));
                sb.Append(" ");
                sb.Append($"<a href=\"{request.Retweeted}\">{GetString(Languages.Tweet.Original)}</a>");
            }
            else if (request.Replying is not null)
            {
                sb.Append(GetString(Languages.Tweet.Reply));
                sb.Append(" ");
                sb.Append($"<a href=\"{request.Source}\">{GetString(Languages.Tweet.Source)}</a>");
                sb.Append("\n");
                sb.Append(GetString(Languages.Tweet.SourceText));
                sb.Append(" ");
                sb.Append($"<a href=\"{request.Replying}\">{GetString(Languages.Tweet.Original)}</a>");
            }
        }
        else
        {
            sb.Append(GetString(Languages.Tweet.SourceText));
            sb.Append(" ");
            sb.Append($"<a href=\"{request.Source}\">{GetString(Languages.Tweet.Original)}</a>");
        }
        sb.Append("\n\n");
        sb.Append(GetString(Languages.Tweet.Join));
        sb.Append("\n");
        sb.Append($"<a href=\"{GetString(Languages.Tweet.ChannelLink)}\">{GetString(Languages.Tweet.LanguageChannel)}</a>");
        
       //get chats 
       var chats = await _db.ChatRegions
           .Where(x=>x.IsActive)
           .Where(x => x.Region.RegionName == request.Region.ToString())
              .Include(x => x.Chat)
           .ToListAsync(cancellationToken);

       await Parallel.ForEachAsync(chats,cancellationToken, async (x, stoppingTokenToken) =>
       {
           try
           {
               
               const int maxLength = 4096;
               var fullMessage = sb.ToString();

               for (int i = 0; i < fullMessage.Length; i += maxLength)
               {
                   var part = fullMessage.Substring(i, Math.Min(maxLength, fullMessage.Length - i));
                   await _client.Bot.SendMessage(
                       chatId: x.Chat.ChatIdentificationNumber,
                       text: part,
                       messageThreadId: x.TopicId ?? null,
                       parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                       linkPreviewOptions: new LinkPreviewOptions()
                       {
                           IsDisabled = true,
                       },
                       cancellationToken: stoppingTokenToken
                   );
               }
           }
           catch (Exception e)
           {
               _logger.LogError(e, "Error sending message to chat {ChatId}", x.Chat.ChatIdentificationNumber);
           }
           
       });
        return Ok();
    }

    private string GetString(string textToGet)
    {
        return _client.Rm.GetString(textToGet, CultureInfo.CreateSpecificCulture(_culture)) ?? textToGet;
    }

    private string RegionConverter(AllowedRegion region)
    {
        return region switch
        {
            AllowedRegion.China=> "zh",
            AllowedRegion.Global => "en",
            AllowedRegion.Italy => "it",
            AllowedRegion.Russia => "ru",
            AllowedRegion.Germany => "de",
            AllowedRegion.Spain => "es",
            AllowedRegion.Portugal => "pt",
            AllowedRegion.Japanese => "jp",
            AllowedRegion.Hebrew => "he", 
            AllowedRegion.France => "fr",
            AllowedRegion.Undefined => "en",
            _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
        };
    }
}