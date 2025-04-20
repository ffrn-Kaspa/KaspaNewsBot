using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.TelegramBot.Settings;

namespace Telegram.TelegramBot.Bot;

public class TelegramStartupService :IHostedService
{
    private readonly TelegramSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramStartupService> _logger;
    

    public TelegramStartupService(IOptions<TelegramSettings> settings,ILogger<TelegramStartupService> logger, IServiceProvider serviceProvider)
    {
        _settings = settings.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope =_serviceProvider.CreateScope();
        var bot = scope.ServiceProvider.GetService<TelegramClient>()!;
        await bot.Setup();
        _logger.LogInformation("Telegram bot started");
    }

    public Task StopAsync(CancellationToken cancellationToken)=> Task.CompletedTask;
    
}