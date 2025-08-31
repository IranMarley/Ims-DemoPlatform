using Ims.DemoPlatform.Core.Enums;
using Ims.DemoPlatform.Core.MessageBus;
using Ims.DemoPlatform.Core.MessageBus.Contracts;
using Ims.DemoPlatform.Core.MessageBus.Events;
using Ims.DemoPlatform.Identity.API.Data;
using Ims.DemoPlatform.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Ims.DemoPlatform.Identity.API.Services;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);
    Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto dto);
    Task<IdentityResult> GeneratePasswordResetTokenAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(ResetDto dto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMessageBus _messageBus;
    private readonly ITokenService _tokenService;
    private readonly RabbitMqOptions _busOpts;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IMessageBus messageBus,
        ITokenService tokenService,
        IOptions<RabbitMqOptions> busOpts)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _messageBus = messageBus;
        _tokenService = tokenService;
        _busOpts = busOpts.Value;
    }

    public async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        // Check if user exists
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return new AuthResult(false, null, ErrorDescription: "Invalid credentials.");

        // Check password
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded) return new AuthResult(false, null, ErrorDescription: "Invalid credentials.");

        // Check if email is confirmed
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return new AuthResult(false, null, ErrorDescription: "Email not confirmed");
        }

        // Check if user is locked out
        if (await _userManager.IsLockedOutAsync(user))
        {
            return new AuthResult(false, null, ErrorDescription: "User is locked out");
        }

        // Issue token pair
        return new AuthResult(true, await _tokenService.IssueTokenPairAsync(user), "");
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
    {
        var identityUser = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            EmailConfirmed = true
        };

        var res = await _userManager.CreateAsync(identityUser, registerDto.Password);

        if (res.Succeeded)
        {
            await _userManager.AddToRoleAsync(identityUser, nameof(DefaultRoles.User));
        
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            
            _messageBus.PublishAsync(nameof(AuthEvents), AuthEvents.UserRegistered,
                new UserRegistered(identityUser.Email, confirmationToken));
        }

        return res;
    }

    public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        var res = await _userManager.ConfirmEmailAsync(user, dto.Token);

        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _messageBus.PublishAsync(nameof(AuthEvents), AuthEvents.UserPasswordResetRequested,
            new PasswordResetRequested(email, token));

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        var res = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (!res.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError
                { Description = string.Join(", ", res.Errors.Select(e => e.Description)) });
        }

        return IdentityResult.Success;
    }
}