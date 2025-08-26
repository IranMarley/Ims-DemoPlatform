namespace Ims.DemoPlatform.Core.MessageBus;

public class RabbitMqOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Exchange { get; set; }
    public bool UseSsl { get; set; }
}