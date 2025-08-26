using Ims.DemoPlatform.Identity.API.Data;
using Ims.DemoPlatform.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ims.DemoPlatform.Identity.API.Services;

public interface IUserService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser> GetUserAsync(string userId);
    Task<IdentityResult> CreateUserAsync(UserDto user);
    Task<IdentityResult> UpdateUserAsync(string userId, UserDto user);
    Task<IdentityResult> DeleteUserAsync(string userId);
}

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public UserService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }
    
    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<ApplicationUser> GetUserAsync(string userId)
    {
        return (await _userManager.FindByIdAsync(userId)) ?? throw new Exception("User not found");
    }

    public async Task<IdentityResult> CreateUserAsync(UserDto user)
    {
        var identityUser = new ApplicationUser
        {
            UserName = user.Email,
            Email = user.Email,
            EmailConfirmed = true,
        };

        var res = await _userManager.CreateAsync(identityUser, user.Password);

        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }
        
        res = await UpdateUserRolesAsync(identityUser, user.Role);
        
        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }
        
        return res;
    }

    public async Task<IdentityResult> UpdateUserAsync(string userId, UserDto user)
    {
        var identityUser = await GetUserAsync(userId);
        if (identityUser == null) throw new Exception("User not found");

        identityUser.Name = user.Name;
        identityUser.UserName = user.Email;
        identityUser.Email = user.Email;

        var res = await _userManager.UpdateAsync(identityUser);
        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }
        
        res = await UpdateUserRolesAsync(identityUser, user.Role);
        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }
        
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var identityUser = await GetUserAsync(userId);
        if (identityUser == null) throw new Exception("User not found");

        var res = await _userManager.DeleteAsync(identityUser);

        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }

        return IdentityResult.Success;
    }
    
    private async Task<IdentityResult> UpdateUserRolesAsync(ApplicationUser identityUser, string roles)
    {
        // Remove current roles
        var currentRoles = await _userManager.GetRolesAsync(identityUser);
        await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);

        // Add new roles
        var result = await _userManager.AddToRoleAsync(identityUser, roles);
        if (!result.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }
        
        return IdentityResult.Success;
    }
}