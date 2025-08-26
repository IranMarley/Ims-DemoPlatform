namespace Ims.DemoPlatform.Core.MessageBus.Contracts;
public record PasswordResetRequested(string Email, string ResetToken);
