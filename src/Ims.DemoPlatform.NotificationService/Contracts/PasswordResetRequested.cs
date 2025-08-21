namespace EmailService.Contracts;
public record PasswordResetRequested(string UserId, string Email, string ResetToken);
