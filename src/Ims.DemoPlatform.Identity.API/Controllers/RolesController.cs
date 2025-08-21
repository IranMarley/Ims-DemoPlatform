using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("roles")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roles;
    private readonly UserManager<Data.ApplicationUser> _users;
    public RolesController(RoleManager<IdentityRole> roles, UserManager<AuthApi.Data.ApplicationUser> users)
    {
        _roles = roles; _users = users;
    }

    [HttpGet] public IActionResult List() => Ok(_roles.Roles.Select(r => new { r.Id, r.Name }));

    [HttpPost] public async Task<IActionResult> Create([FromBody] RoleDto dto)
    {
        var res = await _roles.CreateAsync(new IdentityRole(dto.Name));
        return res.Succeeded ? Ok() : BadRequest(res.Errors);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        var role = await _roles.FindByNameAsync(name);
        if (role is null) return NotFound();
        var res = await _roles.DeleteAsync(role);
        return res.Succeeded ? Ok() : BadRequest(res.Errors);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] RoleAssignDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null) return NotFound("User not found");
        var res = await _users.AddToRoleAsync(user, dto.Role);
        return res.Succeeded ? Ok() : BadRequest(res.Errors);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] RoleAssignDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null) return NotFound("User not found");
        var res = await _users.RemoveFromRoleAsync(user, dto.Role);
        return res.Succeeded ? Ok() : BadRequest(res.Errors);
    }
}

public record RoleDto([Required] string Name);
public record RoleAssignDto([Required, EmailAddress] string Email, [Required] string Role);
