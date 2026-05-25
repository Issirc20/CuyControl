using Microsoft.AspNetCore.Mvc;

namespace CuyControl.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class DashboardApiController : ControllerBase
{
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(new { uptime = "unknown", users = 0 });
    }
}
