using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Db.Enums;
using Telegram.DiscordBot.Model;
using Telegram.DiscordBot.Settings;

namespace Telegram.DiscordBot.Worker;

public class DiscordClient :BackgroundService
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<DiscordClient> _logger;
    private readonly DiscordSettings _settings;
    private readonly HttpClient _webClient;
    private readonly IServiceProvider _provider;
    private bool _isAnswer;
    private const string QuotedPattern = @"\[Quoted\]\((https:\/\/[^\s\)]+)\)";
    public const string RetweetedPattern = @"\[Retweeted\]\((https:\/\/[^\s\)]+)\)";
    public const string ReplyingPattern = @"\[Replying\]\((https:\/\/[^\s\)]+)\)";
    public const string TweetPattern = @"\[@\w+\]\((https:\/\/twitter\.com\/\w+\/status\/\d+)\)";
    public const string TweetPattern2 = @"\[Tweeted\]\((https:\/\/twitter\.com\/\w+\/status\/\d+)\)";
    public const string UserPattern = @"(https:\/\/twitter\.com\/\w+)";
    public const string UserNamePattern = @"(?:https?:\/\/)?twitter\.com\/([\w_]+)";

    public DiscordClient(ILogger<DiscordClient> logger, IOptions<DiscordSettings> settings, HttpClient webClient, IServiceProvider cache)
    {
        _logger = logger;
        _webClient = webClient;
        _provider = cache;
        _settings = settings.Value;
        _isAnswer = false;

        var config = new DiscordSocketConfig
        {

            GatewayIntents = GatewayIntents.Guilds
                             | GatewayIntents.GuildMessages
                             | GatewayIntents.MessageContent
                             | GatewayIntents.GuildMessageReactions,
     
        };
        _client = new DiscordSocketClient(config);
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    
        if (_client.ConnectionState == ConnectionState.Connected)
        {
            _logger.LogInformation("Already connected to Discord");
            return;
        }
        await _client.LoginAsync(TokenType.Bot, _settings.TestToken);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    
    
    
    
    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

 
    private Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");

        return Task.CompletedTask;
    }
    
    
    private async Task<Task> MessageReceivedAsync(SocketMessage message)
    {
        using var scope = _provider.CreateScope();
        IMemoryCache cache = scope.ServiceProvider.GetService<IMemoryCache>()!;
       
        if (message.Author.Id == _client.CurrentUser.Id)
            return Task.CompletedTask;

        _logger.LogInformation("Message from {User}: {Content}", message.Author.Username, message.Content);
        
        var newerMessage = message.Content;
        string? previousPart = cache.TryGetValue("Previous", out string? previous) ? previous : string.Empty;
        if (!string.IsNullOrEmpty(previousPart))
        {
            newerMessage = previousPart + newerMessage;
        }
        
        if (message.Channel is SocketTextChannel textChannel)
        {
            string? regionCode = newerMessage[..2];
            var region = regionCode switch
            {
                "CN" => AllowedRegion.China,
                "GL" => AllowedRegion.Global,
                "IT" => AllowedRegion.Italy,
                "RU" => AllowedRegion.Russia,
                "DE" => AllowedRegion.Germany,
                "ES" => AllowedRegion.Spain,
                "PT" => AllowedRegion.Portugal,
                "JP" => AllowedRegion.Japanese,
                "HE" => AllowedRegion.Hebrew,
                "FR" => AllowedRegion.France,
                _ => AllowedRegion.Undefined
            };
                      
            
            Match quoteMatch = Regex.Match(newerMessage, QuotedPattern);
            var quoted =  String.IsNullOrWhiteSpace(quoteMatch.Groups[1].Value) ? null : quoteMatch.Groups[1].Value;
            
            Match retweetMatch = Regex.Match(newerMessage, RetweetedPattern);
            var retweeted = String.IsNullOrWhiteSpace(retweetMatch.Groups[1].Value) ? null : retweetMatch.Groups[1].Value;
           
            Match replyMatch = Regex.Match(newerMessage, ReplyingPattern);
            var replying = String.IsNullOrWhiteSpace(replyMatch.Groups[1].Value) ? null : replyMatch.Groups[1].Value;
            
            Match usernameMatch = Regex.Match(newerMessage, TweetPattern);
            var tweet = String.IsNullOrWhiteSpace(usernameMatch.Groups[1].Value) ? "Not Found" : usernameMatch.Groups[1].Value;
            if(tweet.Equals("Not Found"))
            {
                Match usernameMatch2 = Regex.Match(newerMessage, TweetPattern2);
                tweet = String.IsNullOrWhiteSpace(usernameMatch2.Groups[1].Value) ? "Not Found" : usernameMatch2.Groups[1].Value;
            }

            var user = "";
           

            if ( tweet.Equals("Not Found"))
            {
                cache.Set("Previous", newerMessage);
                
                return Task.CompletedTask;
            }
            if(quoted is not null)
            {
                Match userMatch = Regex.Match(quoted, UserPattern);
                user = String.IsNullOrWhiteSpace(userMatch.Groups[1].Value) ? "Not Found" : userMatch.Groups[1].Value;
                _isAnswer = true;
            }
            if (retweeted is not null)
            {
                Match userMatch = Regex.Match(retweeted, UserPattern);
                user = String.IsNullOrWhiteSpace(userMatch.Groups[1].Value) ? "Not Found" : userMatch.Groups[1].Value;
                _isAnswer = true;
            }
            if (replying is not null)
            {
                Match userMatch = Regex.Match(replying, UserPattern);
                user = String.IsNullOrWhiteSpace(userMatch.Groups[1].Value) ? "Not Found" : userMatch.Groups[1].Value;
                _isAnswer = true;
            }
         
            var content = newerMessage.Split("\n");
            var cleaned = content.Where(x => x != "CN")
                .Where(x => x != "GL")
                .Where(x => x != "IT")
                .Where(x => x != "RU")
                .Where(x => x != "DE")
                .Where(x => x != "ES")
                .Where(x => x != "PT")
                .Where(x => x != "JP")
                .Where(x => x != "HE")
                .Where(x => x != "FR")
                .Where(x => !x.Contains("[Quoted]("))
                .Where(x => !x.Contains("[Retweeted]("))
                .Where(x => !x.Contains("[Replying]("))
                .Where(x => !x.Contains(tweet));
            var textContent = string.Join("\n", cleaned);
            if (string.IsNullOrEmpty(user))
            {
                Match userMatch = Regex.Match(tweet, UserPattern);
                user = String.IsNullOrWhiteSpace(userMatch.Groups[1].Value) ? "Not Found" : userMatch.Groups[1].Value;
                
            }
            var usernamestringMatch = Regex.Match(user, UserNamePattern);
            var username = String.IsNullOrWhiteSpace(usernamestringMatch.Groups[1].Value) ? "Not Found" : usernamestringMatch.Groups[1].Value;
          
            
            var regex = new Regex(@"<(?<url>https?://[^>]+)>", RegexOptions.Compiled);

            // Sostituisci ogni match con HTML valido per Telegram
            var result = regex.Replace(textContent, match =>
            {
                var url = match.Groups["url"].Value;
                return $"<a href=\"{url}\">{url}</a>";
            });
            cache.Remove("Previous");
            MessageDto newMessage = new()
            {
                Region = region,
                Quoted = quoted,
                Replying = replying,
                Retweeted = retweeted,
                User = user,
                Username = username,
                Message = result,
                Source = tweet,
                IsReply = _isAnswer
            };
          await _webClient.PostAsJsonAsync(_settings.PostUrl, newMessage);
        }
        return Task.CompletedTask;
    }




    
}