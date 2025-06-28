
using Telegram.TelegramBot.Bot;
using Telegram.TelegramBot.Settings;
using Telegram.Db;
using Telegram.DiscordBot.Settings;
using Telegram.DiscordBot.Worker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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


builder.Services.AddTelegramDb(
    builder.Configuration.GetConnectionString(ConnectionStrings.Telegram)!);
builder.Services.AddOptions<TelegramSettings>()
    .BindConfiguration(TelegramSettings.Section)
    .ValidateDataAnnotations()
    .ValidateOnStart();


builder.Services.AddHttpClient();
builder.Services.AddSingleton<TelegramClient>();
builder.Services.AddHostedService<TelegramStartupService>();
builder.Services.AddLocalization();
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();
app.UseRouting();
app.MapControllers();

var supportedCultures = new[]
{
    "zh",
    "en"
};
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures)
);


app.Run();

