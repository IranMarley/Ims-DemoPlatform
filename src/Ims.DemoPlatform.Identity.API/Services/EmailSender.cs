namespace AuthApi.Services;
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody);
}
public class ConsoleEmailSender : IEmailSender
{
    public Task SendAsync(string to, string subject, string htmlBody)
    {
        Console.WriteLine($"[EMAIL DEV] To: {to}\nSubject: {subject}\nBody: {htmlBody}");
        return Task.CompletedTask;
    }
}
