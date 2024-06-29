using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using UserConfirmation.Services.Confirmations;
using System.Text;
using Newtonsoft.Json;
using UserConfirmation.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace UserConfirmation.Services.Worker;
public class ConfirmationWorker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public ConfirmationWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var factory = new ConnectionFactory() { HostName = "localhost" };
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
            var request = JsonConvert.DeserializeObject<ConfirmationRequest>(message);

            if (request != null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var confirmationService = scope.ServiceProvider.GetRequiredService<IConfirmationService>();
                    await confirmationService.GenerateAndSendConfirmationCodeAsync(request.UserId);
                }
            }
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
