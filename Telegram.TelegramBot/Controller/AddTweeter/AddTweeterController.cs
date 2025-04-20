using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Db;
using Telegram.Db.Model;

namespace Telegram.TelegramBot.Controller.AddTweeter;

[ApiController]
[Route("tweeter")]
public class AddTweeterController :ControllerBase
{
    private readonly ILogger<AddTweeterController> _logger;
    private readonly TelegramDbContext _db;
    private static readonly List<string> AllowedRegionCodes = ["ZH", "EN", "IT", "RU", "DE", "ES", "PT", "JP", "HE", "FR"];

    public AddTweeterController(ILogger<AddTweeterController> logger, TelegramDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddTweeterRequestDto request, CancellationToken cancellationToken)
    {
     
    
        if(!AllowedRegionCodes.Contains(request.RegionCode) )
        {
            _logger.LogWarning("Invalid region code: {RegionCode}", request.RegionCode);
            return BadRequest(new { Message = "Invalid region code" });
        }
        //add to db
        var regionId = await _db.AllowedLanguages        
            .Where(x => x.RegionCode == request.RegionCode.ToLower())
            .AsNoTracking()
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        _logger.LogInformation("Received new tweeter request: {@Request}", request);
        var newTweeter = new TweeterDbo
        {
            RegionId = regionId,
            Name = request.TweeterName,
            Link = request.TweeterLink,
            Username = request.TweeterUsername,
        };
        await _db.Tweeters.AddAsync(newTweeter, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(new { Message = "Tweeter processed successfully", Request = request });
    }
    
}