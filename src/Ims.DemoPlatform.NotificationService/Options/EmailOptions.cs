namespace EmailService.Options;
public class EmailOptions
{
    public string From { get; set; } = "no-reply@example.com";
    public string Provider { get; set; } = "Console"; // or "Smtp"
}
