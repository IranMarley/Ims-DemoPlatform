namespace EmailService.Options;
public class SmtpOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; } = false;
}
