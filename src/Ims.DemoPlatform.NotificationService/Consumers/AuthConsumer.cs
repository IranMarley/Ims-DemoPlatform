using Ims.DemoPlatform.Core.MessageBus;
using Ims.DemoPlatform.Core.MessageBus.Contracts;
using Ims.DemoPlatform.Core.MessageBus.Events;
using Ims.DemoPlatform.NotificationService.Model;
using Ims.DemoPlatform.NotificationService.Options;
using Ims.DemoPlatform.NotificationService.Services;
using Microsoft.Extensions.Options;

namespace Ims.DemoPlatform.NotificationService.Consumers;

public class AuthConsumer : BackgroundService
{
    private readonly ILogger<AuthConsumer> _logger;
    private readonly ClientOptions _clientOptions;
    private readonly IMessageBus _bus;
    private readonly IEmailSender _email;

    public AuthConsumer(ILogger<AuthConsumer> logger, IOptions<ClientOptions> clientOptions, IMessageBus bus, IEmailSender email)
    {
        _logger = logger;
        _clientOptions = clientOptions.Value;
        _bus = bus;
        _email = email;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            const string exchange = nameof(AuthEvents);
        
            var t1 = _bus.ConsumeAsync<UserRegistered>(exchange, AuthEvents.UserRegistered, HandleUserRegistered, stoppingToken);
            var t2 = _bus.ConsumeAsync<PasswordResetRequested>(exchange, AuthEvents.UserPasswordResetRequested, HandlePasswordResetRequested, stoppingToken);

            Console.WriteLine("[*] NotificationService is running. Waiting for messages...");

            await Task.WhenAll(t1, t2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AuthConsumer");
        }
    }

    private Task HandleUserRegistered(UserRegistered evt)
    {
        var data = new EmailData(
            subject: "Confirm your email",
            preheader: "Please confirm your email to complete your registration.",
            email_tag: "welcome",
            title: "Welcome to DemoPlatform!",
            name: evt.Email,
            body_html: "<p>Thanks for signing up. Please confirm your email.</p>",
            body_text: "Thanks for signing up. Please confirm your email.",
            cta_text: "Confirm Email",
            cta_url: $"{_clientOptions.Url}/confirm-email?email={evt.Email}&token={evt.ConfirmationToken}",
            brand_name: "DemoPlatform",
            brand_url: $"https://4m4you.com/wp-content/uploads/2020/06/logo-placeholder.png",
            brand_logo: $"{_clientOptions.Url}/logo.png",
            brand_address: "123 Demo St, Demo City, DM 12345",
            support_url: $"{_clientOptions.Url}/support",
            unsubscribe_url: null,
            legal_reason: "You are receiving this email because you signed up for DemoPlatform."
        );
        
        return _email.SendAsync(evt.Email, data);
    }

    private Task HandlePasswordResetRequested(PasswordResetRequested evt)
    {
       var emailData = new EmailData(
            subject: "Reset your password",
            preheader: "Click the link below to reset your password.",
            email_tag: "password-reset",
            title: "Password Reset Request",
            name: evt.Email,
            body_html: @"
                <p>We received a request to reset your password.</p>
                <p>If you did not make this request, please ignore this email.</p>",
            body_text: @"
                We received a request to reset your password.
                If you did not make this request, please ignore this email.",
            cta_text: "Reset Password",
            cta_url: $"https://demo-platform.com/reset-password?email={evt.Email}&token={evt.ResetToken}",
            brand_name: "DemoPlatform",
            brand_url: $"https://4m4you.com/wp-content/uploads/2020/06/logo-placeholder.png",
            brand_logo: $"{_clientOptions.Url}/logo.png",
            brand_address: "123 Demo St, Demo City, DM 12345",
            support_url: $"{_clientOptions.Url}/support",
            unsubscribe_url: null,
            legal_reason: "You are receiving this email because you requested a password reset."
        );

        return _email.SendAsync(evt.Email, emailData);
    }
}