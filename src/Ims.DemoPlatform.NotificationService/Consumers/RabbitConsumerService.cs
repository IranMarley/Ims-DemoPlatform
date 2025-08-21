using System.Text;
using System.Text.Json;
using EmailService.Contracts;
using EmailService.Options;
using EmailService.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailService.Consumers;

public class RabbitConsumerService : BackgroundService
{
    private readonly RabbitOptions _opt;
    private readonly IEmailSender _email;

    private IConnection? _conn;
    private IModel? _ch;

    public RabbitConsumerService(IOptions<RabbitOptions> opt, IEmailSender email)
    {
        _opt = opt.Value;
        _email = email;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Resilient connect loop
        var factory = new ConnectionFactory
        {
            HostName = _opt.Host,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };

        int attempt = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _conn = factory.CreateConnection();
                _ch = _conn.CreateModel();

                // Topology
                _ch.ExchangeDeclare(_opt.Exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);

                _ch.QueueDeclare("email.user.registered", durable: true, exclusive: false, autoDelete: false);
                _ch.QueueBind("email.user.registered", _opt.Exchange, "user.registered");

                _ch.QueueDeclare("email.password.reset", durable: true, exclusive: false, autoDelete: false);
                _ch.QueueBind("email.password.reset", _opt.Exchange, "password.reset.requested");

                // Prefetch for fair dispatch
                _ch.BasicQos(0, prefetchCount: 10, global: false);

                // Start consumers
                Consume<UserRegistered>("email.user.registered", HandleUserRegistered, stoppingToken);
                Consume<PasswordResetRequested>("email.password.reset", HandlePasswordResetRequested, stoppingToken);

                Console.WriteLine("[*] Ims.DemoPlatform.NotificationService is running. Waiting for messages...");
                // Wait here until cancelled; consumers will run in background
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
                break;
            }
            catch (Exception ex)
            {
                attempt++;
                Console.WriteLine($"[RabbitMQ] connect/consume failed (attempt {attempt}): {ex.Message}");
                // backoff and retry
                await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, attempt * 2)), stoppingToken);
            }
        }
    }

    private void Consume<T>(string queue, Func<T, Task> handler, CancellationToken ct)
    {
        if (_ch is null) throw new InvalidOperationException("Channel not initialized");

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.Received += async (s, ea) =>
        {
            try
            {
                if (ct.IsCancellationRequested) return;

                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var obj = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (obj is null) throw new Exception("Deserialization returned null");

                await handler(obj);
                _ch.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                // send to DLQ in real world; here we NACK (no requeue) to avoid poison loops
                _ch.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _ch.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        Console.WriteLine($"[*] Consuming {queue}");
    }

    private Task HandleUserRegistered(UserRegistered evt)
    {
        var subject = "Confirm your email";
        var body = $"<h2>Welcome!</h2><p>Thanks for signing up. Please confirm your email.</p><p>User: {evt.Email}</p>";
        return _email.SendAsync(evt.Email, subject, body);
    }

    private Task HandlePasswordResetRequested(PasswordResetRequested evt)
    {
        var subject = "Password reset";
        var body = $"<h2>Reset your password</h2><p>Use this token: <b>{evt.ResetToken}</b></p>";
        return _email.SendAsync(evt.Email, subject, body);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _ch?.Close();
            _conn?.Close();
        }
        catch { /* ignore on shutdown */ }
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        try
        {
            _ch?.Dispose();
            _conn?.Dispose();
        }
        catch { /* ignore on dispose */ }
        base.Dispose();
    }
}
