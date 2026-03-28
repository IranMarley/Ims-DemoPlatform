using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Tasks.API.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "Healthy" });
}

