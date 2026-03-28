using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Projects.API.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "Healthy" });
}

