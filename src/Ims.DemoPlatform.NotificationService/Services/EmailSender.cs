using EmailService.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EmailService.Services;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody);
}

public class ConsoleEmailSender : IEmailSender
{
    private readonly EmailOptions _opt;
    public ConsoleEmailSender(IOptions<EmailOptions> opt) => _opt = opt.Value;
    public Task SendAsync(string to, string subject, string htmlBody)
    {
        Console.WriteLine($"[EMAIL DEV] To: {to}\nSubject: {subject}\nBody: {htmlBody}");
        return Task.CompletedTask;
    }
}

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _email;
    private readonly SmtpOptions _smtp;
    public SmtpEmailSender(IOptions<EmailOptions> email, IOptions<SmtpOptions> smtp)
    {
        _email = email.Value; _smtp = smtp.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        using var client = new SmtpClient(_smtp.Host, _smtp.Port) { EnableSsl = _smtp.EnableSsl };
        if (!string.IsNullOrWhiteSpace(_smtp.User))
            client.Credentials = new NetworkCredential(_smtp.User, _smtp.Password);

        var mail = new MailMessage(_email.From, to, subject, htmlBody) { IsBodyHtml = true };
        await client.SendMailAsync(mail);
    }
}
