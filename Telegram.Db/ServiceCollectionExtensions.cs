using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Db;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<TelegramDbContext>(options =>
            options
                .UseNpgsql(connectionString, o => { o.MigrationsAssembly("Telegram.Migrations"); })
                .UseSnakeCaseNamingConvention()
        );
    }
}