namespace EmailService.Options;
public class RabbitOptions
{
    public string Host { get; set; } = "localhost";
    public string Exchange { get; set; } = "auth.events";
}
