using Ims.DemoPlatform.Identity.API.Models;
using Ims.DemoPlatform.Identity.API.Services;
using Ims.DemoPlatform.WebApi.Core.Extensions;
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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        _logger.LogInformation("User registration attempt for email: {Email}", dto.Email);
        
        var res = await _authService.RegisterAsync(dto);
    
        if (!res.Succeeded)
        {
            _logger.LogWarning("Registration failed for email: {Email}. Errors: {Errors}", 
                dto.Email, string.Join(", ", res.GetErrors()));
            return BadRequest(this.Fail(res.GetErrors()));
        }
        
        _logger.LogInformation("User successfully registered: {Email}", dto.Email);
        return Ok(this.Success());
    }
    
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
    {
        _logger.LogInformation("Email confirmation attempt for user: {Email}", dto.Email);
        
        var res = await _authService.ConfirmEmailAsync(dto);
        
        if (!res.Succeeded)
        {
            _logger.LogWarning("Email confirmation failed for user: {Email}", dto.Email);
            return BadRequest(this.Fail(res.GetErrors()));
        }
        
        _logger.LogInformation("Email confirmed successfully for user: {Email}", dto.Email);
        return Ok(this.Success());
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("Login attempt for email: {Email} from IP: {IpAddress}", 
            dto.Email, HttpContext.Connection.RemoteIpAddress);
        
        var res = await _authService.AuthenticateAsync(dto.Email, dto.Password);
    
        if (!res.Succeeded)
        {
            _logger.LogWarning("Failed login attempt for email: {Email} from IP: {IpAddress}. Reason: {Reason}", 
                dto.Email, HttpContext.Connection.RemoteIpAddress, res.ErrorDescription);
            return BadRequest(this.Fail(res.ErrorDescription));
        }
        
        _logger.LogInformation("Successful login for email: {Email} from IP: {IpAddress}", 
            dto.Email, HttpContext.Connection.RemoteIpAddress);
        return Ok(this.Success(res.Tokens));
    }
    
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh([FromBody] RefreshDto dto)
    {
        _logger.LogInformation("Token refresh attempt from IP: {IpAddress}", 
            HttpContext.Connection.RemoteIpAddress);
        
        var res = await _tokenService.RefreshAsync(dto.RefreshToken);
        
        if (res is null)
        {
            _logger.LogWarning("Token refresh failed - invalid or expired refresh token from IP: {IpAddress}", 
                HttpContext.Connection.RemoteIpAddress);
            return Unauthorized(this.Fail());
        }
        
        _logger.LogInformation("Token refreshed successfully from IP: {IpAddress}", 
            HttpContext.Connection.RemoteIpAddress);
        return Ok(this.Success(res));
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout([FromBody] RefreshDto dto)
    {
        var userEmail = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Logout attempt for user: {UserEmail}", userEmail);
        
        var res = await _tokenService.RevokeAsync(dto.RefreshToken);
        
        if (!res)
        {
            _logger.LogWarning("Logout failed - token not found for user: {UserEmail}", userEmail);
            return NotFound(this.Fail());
        }
        
        _logger.LogInformation("User logged out successfully: {UserEmail}", userEmail);
        return Ok(this.Success());
    }
    
    [HttpPost("forgot")]
    [AllowAnonymous]
    public async Task<ActionResult> Forgot([FromBody] ForgotDto dto)
    {
        _logger.LogInformation("Password reset requested for email: {Email} from IP: {IpAddress}", 
            dto.Email, HttpContext.Connection.RemoteIpAddress);
        
        var res = await _authService.GeneratePasswordResetTokenAsync(dto.Email);
        
        // Always return success to prevent email enumeration
        // Log the actual result internally
        if (res.Succeeded)
        {
            _logger.LogInformation("Password reset token generated for email: {Email}", dto.Email);
        }
        else
        {
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", dto.Email);
        }
        
        return Ok(this.Success());
    }
    
    [HttpPost("reset")]
    [AllowAnonymous]
    public async Task<ActionResult> Reset([FromBody] ResetDto dto)
    {
        _logger.LogInformation("Password reset attempt for email: {Email}", dto.Email);
        
        var res = await _authService.ResetPasswordAsync(dto);
    
        if (!res.Succeeded)
        {
            _logger.LogWarning("Password reset failed for email: {Email}. Errors: {Errors}", 
                dto.Email, string.Join(", ", res.GetErrors()));
            return BadRequest(this.Fail(res.GetErrors()));
        }
        
        _logger.LogInformation("Password reset successful for email: {Email}", dto.Email);
        return Ok(this.Success());
    }
}
