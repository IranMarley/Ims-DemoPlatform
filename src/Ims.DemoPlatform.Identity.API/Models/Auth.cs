using System.ComponentModel.DataAnnotations;

namespace Ims.DemoPlatform.Identity.API.Models;

public record RegisterDto([Required] string Name, [Required, EmailAddress] string Email, [Required] string Password);
public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);
public record RefreshDto([Required] string RefreshToken);
public record ConfirmEmailDto([Required, EmailAddress] string Email, [Required] string Token);
public record ForgotDto([Required, EmailAddress] string Email);
public record ResetDto([Required] string Email, [Required] string Token, [Required] string NewPassword);
public record TokenPair(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
public record AuthResult(bool Succeeded, TokenPair? Tokens = null, string? ErrorDescription = null);
