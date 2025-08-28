using Ims.DemoPlatform.Core.Extensions;
using Ims.DemoPlatform.Identity.API.Models;
using Ims.DemoPlatform.Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Identity.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService, ITokenService tokenService)
    {
        _logger = logger;
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("register")][AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var res = await _authService.RegisterAsync(dto);

        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        return Ok(this.Success());
    }

    [HttpPost("confirm-email")][AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
    {
        var res = await _authService.ConfirmEmailAsync(dto);;
        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        return Ok(this.Success());
    }

    [HttpPost("login")][AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {
        var res = await _authService.AuthenticateAsync(dto.Email, dto.Password);
        
        if (!res.Succeeded) return BadRequest(this.Fail(res.ErrorDescription));
        return Ok(this.Success(res.Tokens));
    }

    [HttpPost("refresh")][AllowAnonymous]
    public async Task<ActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var res = await _tokenService.RefreshAsync(dto.RefreshToken);
        return res is null ? Unauthorized() : Ok(this.Success(res));
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] RefreshDto dto)
    {
        var res = await _tokenService.RevokeAsync(dto.RefreshToken);
        return res ? Ok() : NotFound();
    }

    [HttpPost("forgot")][AllowAnonymous]
    public async Task<ActionResult> Forgot([FromBody] ForgotDto dto)
    {
        var res = await _authService.GeneratePasswordResetTokenAsync(dto.Email);
        
        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        return Ok();
    }

    [HttpPost("reset")][AllowAnonymous]
    public async Task<ActionResult> Reset([FromBody] ResetDto dto)
    {
        var res = await _authService.ResetPasswordAsync(dto);
        
        if (!res.Succeeded) return BadRequest(this.Fail(res.GetErrors()));
        return Ok();
    }
}