using Ims.DemoPlatform.Identity.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ims.DemoPlatform.Identity.API.Services;

public interface IRoleService
{
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task<IdentityRole> GetRoleAsync(string roleId);
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> UpdateRoleAsync(string roleId, string roleName);
    Task<IdentityResult> DeleteRoleAsync(string roleId);
}

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<IdentityRole> GetRoleAsync(string roleId)
    {
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role already exists." });
        }

        var role = new IdentityRole(roleName);
        return await _roleManager.CreateAsync(role);
    }

    public async Task<IdentityResult> UpdateRoleAsync(string roleId, string roleName)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
        }

        role.Name = roleName;
        return await _roleManager.UpdateAsync(role);
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
        {
            return IdentityResult.Failed(new IdentityError { Description = "Cannot delete role with assigned users." });
        }

        return await _roleManager.DeleteAsync(role);
    }
}