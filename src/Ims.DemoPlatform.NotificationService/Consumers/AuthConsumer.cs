using Ims.DemoPlatform.Core.MessageBus;
using Ims.DemoPlatform.Core.MessageBus.Contracts;
using Ims.DemoPlatform.Core.MessageBus.Events;
using Ims.DemoPlatform.NotificationService.Services;

namespace Ims.DemoPlatform.NotificationService.Consumers;

public class AuthConsumer : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly IEmailSender _email;

    public AuthConsumer(IMessageBus bus, IEmailSender email)
    {
        _bus = bus;
        _email = email;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var t1 = _bus.ConsumeAsync<UserRegistered>(AuthEvents.UserRegistered, HandleUserRegistered, stoppingToken);
        var t2 = _bus.ConsumeAsync<PasswordResetRequested>(AuthEvents.UserPasswordResetRequested, HandlePasswordResetRequested, stoppingToken);

        Console.WriteLine("[*] NotificationService is running. Waiting for messages...");

        await Task.WhenAll(t1, t2);
    }

    private Task HandleUserRegistered(UserRegistered evt)
    {
        var subject = "Confirm your email";
        var body = $@"
            <h2>Welcome!</h2>
            <p>Thanks for signing up. Please confirm your email.</p>
            <p>User: {evt.Email}</p>";

        return _email.SendAsync(evt.Email, subject, body);
    }

    private Task HandlePasswordResetRequested(PasswordResetRequested evt)
    {
        var subject = "Password reset";
        var body = $@"
            <h2>Reset your password</h2>
            <p>Use this token: <b>{evt.ResetToken}</b></p>";

        return _email.SendAsync(evt.Email, subject, body);
    }
}