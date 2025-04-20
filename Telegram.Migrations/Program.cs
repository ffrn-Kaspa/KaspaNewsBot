using Telegram.Db;
using Telegram.Db.Model;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelegramDb(
    builder.Configuration.GetConnectionString(ConnectionStrings.Telegram)!);

var app = builder.Build();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<TelegramDbContext>();

   var first = new RegionDbo
    {
        Id = Guid.NewGuid(),
        RegionCode = "en",
        RegionName = "Global"
    };
    var second = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "ru",
          RegionName = "Russia"
     };
    var third = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "it",
          RegionName = "Italy"
     };
    var fourth = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "es",
          RegionName = "Spain"
     };
    var fifth = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "de",
          RegionName = "Germany"
     };
    var sixth = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "pt",
          RegionName = "Portugal"
     };
    var seventh = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "jp",
          RegionName = "Japanese"
     };
    var eighth = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "he",
          RegionName = "Hebrew"
     };
    var ninth = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "zh",
          RegionName = "China"
     };
    var tenth = new RegionDbo
     {
          Id = Guid.NewGuid(),
          RegionCode = "fr",
          RegionName = "France"
     };
    await dbContext.AllowedLanguages.AddRangeAsync(
        first, second, third, fourth, fifth, sixth, seventh, eighth, ninth, tenth);
    await dbContext.SaveChangesAsync();
app.Run();