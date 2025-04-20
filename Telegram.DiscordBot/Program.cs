using Telegram.DiscordBot.Settings;
using Telegram.DiscordBot.Worker;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOptions<DiscordSettings>()
    .BindConfiguration(DiscordSettings.Section)
    .ValidateDataAnnotations()
    .ValidateOnStart();


builder.Services.AddHttpClient();
builder.Services.AddHostedService<DiscordClient>();
builder.Services.AddMemoryCache(options =>
{
    options.ExpirationScanFrequency = TimeSpan.FromSeconds(10);
});
var app = builder.Build();



app.Run();

