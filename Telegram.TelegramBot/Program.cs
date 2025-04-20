
using Telegram.TelegramBot.Bot;
using Telegram.TelegramBot.Settings;
using Telegram.Db;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

