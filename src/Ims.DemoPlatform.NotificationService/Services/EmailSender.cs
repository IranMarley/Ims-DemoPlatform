using Ims.DemoPlatform.NotificationService.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ims.DemoPlatform.NotificationService.Services;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _smtp;
    public SmtpEmailSender(IOptions<SmtpOptions> smtp)
    {
        _smtp = smtp.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.Name, _smtp.User));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var body = new BodyBuilder { HtmlBody = htmlBody, TextBody = "View as HTML." };
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();

        var secure = _smtp.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(_smtp.Host, _smtp.Port, secure);
        await client.AuthenticateAsync(_smtp.User, _smtp.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
