using Ims.DemoPlatform.Identity.API.Models;
using Ims.DemoPlatform.Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Identity.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService _authService, ITokenService _tokenService)
    {
        _authService = _authService;
        _tokenService = _tokenService;
    }

    [HttpPost("register")][AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var res = await _authService.RegisterAsync(dto);
        if (!res.Succeeded) return BadRequest(res.Errors);
        return Ok();
    }

    [HttpPost("confirm-email")][AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
    {
        var res = await _authService.ConfirmEmailAsync(dto);;
        if (!res.Succeeded) return BadRequest(res.Errors);
        return Ok();
    }

    [HttpPost("login")][AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var res = await _authService.AuthenticateAsync(dto.Email, dto.Password);
        if (!res.Succeeded) return Unauthorized(res.ErrorDescription);
        
        return Ok(new 
        {
            accessToken = res.Tokens!.AccessToken,
            refreshToken = res.Tokens.RefreshToken
        });
    }

    [HttpPost("refresh")][AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var res = await _tokenService.RefreshAsync(dto.RefreshToken);
        return res is null ? Unauthorized() : Ok(new 
        {
            accessToken = res.AccessToken,
            refreshToken = res.RefreshToken
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshDto dto)
    {
        var res = await _tokenService.RevokeAsync(dto.RefreshToken);
        return res ? Ok() : NotFound();
    }

    [HttpPost("forgot")][AllowAnonymous]
    public async Task<IActionResult> Forgot([FromBody] ForgotDto dto)
    {
        var res = await _authService.GeneratePasswordResetTokenAsync(dto.Email);
        if (!res.Succeeded) return BadRequest(res.Errors);
        
        return Ok();
    }

    [HttpPost("reset")][AllowAnonymous]
    public async Task<IActionResult> Reset([FromBody] ResetDto dto)
    {
        var res = await _authService.ResetPasswordAsync(dto);
        return res.Succeeded ? Ok() : BadRequest(res.Errors);
    }
}