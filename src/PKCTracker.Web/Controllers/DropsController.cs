using Microsoft.AspNetCore.Mvc;
using PKCTracker.Web.Data;
using PKCTracker.Web.Interfaces.Services;

namespace PKCTracker.Web.Controllers;

[ApiController]
[Route("api/guilds/{guildId}/drops")]
public class DropsController : ControllerBase
{
    private readonly IDropService _dropService;

    public DropsController(IDropService dropService)
    {
        _dropService = dropService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDropsByGuildIdAsync([FromRoute] ulong guildId)
    {
        var drops = await _dropService.GetDropsByGuildAsync(guildId);

        return Ok(drops);
    }
    
}