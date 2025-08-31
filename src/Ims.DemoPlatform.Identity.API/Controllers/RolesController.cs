using Ims.DemoPlatform.Core.Enums;
using Ims.DemoPlatform.Identity.API.Services;
using Ims.DemoPlatform.WebApi.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Identity.API.Controllers;

[ApiController]
[Route("roles")]
[Authorize(Roles = nameof(DefaultRoles.Admin))]
public class RolesController : ControllerBase
{
    private readonly ILogger<RolesController> _logger;
    private readonly IRoleService _roleService;

    public RolesController(ILogger<RolesController> logger, IRoleService roleService)
    {
        _logger = logger;
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(this.Success(roles));
    }
    
    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRole(string roleId)
    {
        var res = await _roleService.GetRoleAsync(roleId);
        if (res == null) return NotFound(this.Fail("Role not found."));
        
        return Ok(this.Success(res));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var res = await _roleService.CreateRoleAsync(roleName);
        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        
        return Ok(this.Success());
    }
    
    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateRole(string roleId, [FromBody] string roleName)
    {
        var res = await _roleService.UpdateRoleAsync(roleId, roleName);
        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        
        return Ok(this.Success());
    }
    
    [HttpDelete("{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var res = await _roleService.DeleteRoleAsync(roleId);
        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        
        return Ok(this.Success());
    }
}