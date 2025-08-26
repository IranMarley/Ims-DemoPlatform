namespace Ims.DemoPlatform.NotificationService.Options;
public class SmtpOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; }
}
