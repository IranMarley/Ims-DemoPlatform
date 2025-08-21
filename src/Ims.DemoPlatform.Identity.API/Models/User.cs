using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public record RegisterDto([Required, EmailAddress] string Email, [Required] string Password);
public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);
public record RefreshDto([Required] string RefreshToken);
public record ConfirmEmailDto([Required] string UserId, [Required] string Token);
public record ForgotDto([Required, EmailAddress] string Email);
public record ResetDto([Required, EmailAddress] string Email, [Required] string Token, [Required] string NewPassword);
