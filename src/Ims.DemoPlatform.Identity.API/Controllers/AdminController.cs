using System.ComponentModel.DataAnnotations;
using Ims.DemoPlatform.Identity.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Identity.API.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    public AdminController(UserManager<ApplicationUser> users) => _users = users;

    [HttpPost("unlock")]
    public async Task<IActionResult> Unlock([FromBody] UnlockDto dto)
    {
        var u = await _users.FindByEmailAsync(dto.Email);
        if (u is null) return NotFound();
        await _users.SetLockoutEndDateAsync(u, DateTimeOffset.UtcNow);
        await _users.ResetAccessFailedCountAsync(u);
        return Ok(new { message = "User unlocked." });
    }
}

public record UnlockDto([Required, EmailAddress] string Email);
