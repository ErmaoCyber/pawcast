using Microsoft.AspNetCore.Mvc;

namespace PawCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "PawCast.Api",
            status = "ok",
            utcNow = DateTimeOffset.UtcNow
        });
    }
}