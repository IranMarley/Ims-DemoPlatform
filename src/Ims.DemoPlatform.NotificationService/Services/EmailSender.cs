using System.Text.Json;
using HandlebarsDotNet;
using Ims.DemoPlatform.NotificationService.Model;
using Ims.DemoPlatform.NotificationService.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ims.DemoPlatform.NotificationService.Services;

public interface IEmailSender
{
    Task SendAsync(string to, EmailData emailData);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _smtp;
    public SmtpEmailSender(IOptions<SmtpOptions> smtp)
    {
        _smtp = smtp.Value;
    }

    public async Task SendAsync(string to, EmailData emailData)
    {
        var layoutHtml = await File.ReadAllTextAsync(Path.Combine("Templates", "layout.html"));
        var plainTxt   = await File.ReadAllTextAsync(Path.Combine("Templates", "plain.txt"));

        var htmlTemplate = Handlebars.Compile(layoutHtml);
        var textTemplate = Handlebars.Compile(plainTxt);

        var json = JsonSerializer.Serialize(emailData);
        var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(json)!;

        var html = htmlTemplate(data);
        var text = textTemplate(data);
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.Name, _smtp.User));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = data["subject"]?.ToString() ?? "(no subject)";

        var body = new BodyBuilder { HtmlBody = html, TextBody = text };
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();

        var secure = _smtp.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(_smtp.Host, _smtp.Port, secure);
        await client.AuthenticateAsync(_smtp.User, _smtp.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
