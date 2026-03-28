using Ims.DemoPlatform.Core.Enums;
using Ims.DemoPlatform.Identity.API.Models;
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
        var adminUser = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Admin user {AdminUser} requested all roles list", adminUser);
    
        var roles = await _roleService.GetAllRolesAsync();
    
        _logger.LogInformation("Retrieved {RoleCount} roles for admin user {AdminUser}", 
            roles?.Count() ?? 0, adminUser);
    
        return Ok(this.Success(roles));
    }
    
    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRole(string roleId)
    {
        var adminUser = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Admin user {AdminUser} requested role details for roleId: {RoleId}", 
            adminUser, roleId);
    
        var res = await _roleService.GetRoleAsync(roleId);
    
        if (res == null)
        {
            _logger.LogWarning("Role not found: {RoleId} - requested by admin user {AdminUser}", 
                roleId, adminUser);
            return NotFound(this.Fail("Role not found."));
        }
    
        _logger.LogInformation("Role {RoleId} retrieved successfully by admin user {AdminUser}", 
            roleId, adminUser);
    
        return Ok(this.Success(res));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
    {
        var adminUser = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Admin user {AdminUser} attempting to create role: {RoleName} from IP: {IpAddress}", 
            adminUser, dto.Name, HttpContext.Connection.RemoteIpAddress);
    
        var res = await _roleService.CreateRoleAsync(dto.Name);
    
        if (!res.Succeeded)
        {
            _logger.LogWarning("Role creation failed for {RoleName} by admin user {AdminUser}. Errors: {Errors}",
                dto.Name, adminUser, string.Join(", ", res.GetErrors()));
            return BadRequest(this.Fail(res.GetErrors()));
        }
    
        _logger.LogInformation("Role {RoleName} created successfully by admin user {AdminUser}", 
            dto.Name, adminUser);
    
        return Ok(this.Success());
    }
    
    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateRole(string roleId, [FromBody] RoleDto dto)
    {
        var adminUser = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Admin user {AdminUser} attempting to update role {RoleId} to name: {RoleName} from IP: {IpAddress}",
            adminUser, roleId, dto.Name, HttpContext.Connection.RemoteIpAddress);
    
        var res = await _roleService.UpdateRoleAsync(roleId, dto.Name);
    
        if (!res.Succeeded)
        {
            _logger.LogWarning("Role update failed for {RoleId} by admin user {AdminUser}. Errors: {Errors}",
                roleId, adminUser, string.Join(", ", res.GetErrors()));
            return BadRequest(this.Fail(res.GetErrors()));
        }
    
        _logger.LogInformation("Role {RoleId} updated successfully to {RoleName} by admin user {AdminUser}",
            roleId, dto.Name, adminUser);
    
        return Ok(this.Success());
    }
    
    [HttpDelete("{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var adminUser = User.Identity?.Name ?? "Unknown";
        _logger.LogWarning("Admin user {AdminUser} attempting to DELETE role {RoleId} from IP: {IpAddress}",
            adminUser, roleId, HttpContext.Connection.RemoteIpAddress);
    
        var res = await _roleService.DeleteRoleAsync(roleId);
    
        if (!res.Succeeded)
        {
            _logger.LogWarning("Role deletion failed for {RoleId} by admin user {AdminUser}. Errors: {Errors}",
                roleId, adminUser, string.Join(", ", res.GetErrors()));
            return BadRequest(this.Fail(res.GetErrors()));
        }
    
        _logger.LogWarning("Role {RoleId} DELETED by admin user {AdminUser}",
            roleId, adminUser);
    
        return Ok(this.Success());
    }
}
