using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UserConfirmation.Data.Models;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Worker;
public class ConfirmationWorker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqSettings _rabbitMqSettings;

    public ConfirmationWorker(IServiceProvider serviceProvider, IOptions<RabbitMqSettings> options)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqSettings = options.Value;
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqSettings.HostName,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password,
            Port = _rabbitMqSettings.Port,
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "confirmationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            message = message.Replace("\"", "")
                    .Replace("\\", ""); 
            var parts = message.Split(':');
            var userId = parts[0];
            var confirmationCode = parts[1];

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Data.DbContext>();
            var code = new ConfirmationCode(userId, confirmationCode);
            context.ConfirmationCodes.Add(code);
            context.SaveChanges();
        };

        _channel.BasicConsume(queue: "confirmationQueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
