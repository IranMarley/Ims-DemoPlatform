using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Options;
using AuthApi.Services;
using Ims.DemoPlatform.Core.MessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly TokenService _tokens;
    private readonly IMessageBus _bus;
    private readonly RabbitMqOptions _busOpts;

    public AuthController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, TokenService tokens, IEmailSender email, IMessageBus bus, IOptions<RabbitMqOptions> busOpts)
    {
        _users = users; _signIn = signIn; _tokens = tokens; _bus = bus; _busOpts = busOpts.Value;
    }

    [HttpPost("register")][AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var res = await _users.CreateAsync(user, dto.Password);
        if (!res.Succeeded) return BadRequest(res.Errors);

        var token = await _users.GenerateEmailConfirmationTokenAsync(user);
        // Publish event to Email Service
        _bus.Publish(_busOpts.Exchange, "user.registered", new { userId = user.Id, email = user.Email, locale = "en-US" });
        return Ok(new { message = "User created. Confirmation email event published.", userId = user.Id, devEmailToken = token });
    }

    [HttpPost("confirm-email")][AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
    {
        var user = await _users.FindByIdAsync(dto.UserId);
        if (user is null) return NotFound();
        var res = await _users.ConfirmEmailAsync(user, dto.Token);
        return res.Succeeded ? Ok(new { message = "Email confirmed." }) : BadRequest(res.Errors);
    }

    [HttpPost("login")][AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized();

        if (!await _users.IsEmailConfirmedAsync(user)) return Unauthorized("Email not confirmed.");
        if (await _users.IsLockedOutAsync(user)) return Unauthorized("User is locked out.");

        var result = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded) return Unauthorized();

        var pair = await _tokens.IssueTokenPairAsync(user);
        return Ok(new { accessToken = pair.access, refreshToken = pair.refresh });
    }

    [HttpPost("refresh")][AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var pair = await _tokens.RefreshAsync(dto.RefreshToken);
        return pair is null ? Unauthorized() : Ok(new { accessToken = pair.Value.access, refreshToken = pair.Value.refresh });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshDto dto)
    {
        var ok = await _tokens.RevokeAsync(dto.RefreshToken);
        return ok ? Ok(new { message = "Logged out." }) : NotFound();
    }

    [HttpPost("forgot")][AllowAnonymous]
    public async Task<IActionResult> Forgot([FromBody] ForgotDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null) return Ok();
        var token = await _users.GeneratePasswordResetTokenAsync(user);
        // Publish password reset event
        _bus.Publish(_busOpts.Exchange, "password.reset.requested", new { userId = user.Id, email = user.Email, resetToken = token });
        return Ok(new { message = "If the email exists, a reset link was sent." });
    }

    [HttpPost("reset")][AllowAnonymous]
    public async Task<IActionResult> Reset([FromBody] ResetDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null) return NotFound();
        var res = await _users.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        return res.Succeeded ? Ok(new { message = "Password reset." }) : BadRequest(res.Errors);
    }
}