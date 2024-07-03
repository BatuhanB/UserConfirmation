using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.MessageQueue;
public class MessageQueueService : IMessageQueueService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqSettings _rabbitMqSettings;
    public MessageQueueService(IOptions<RabbitMqSettings> options)
    {
        _rabbitMqSettings = options.Value;
        var factory = new ConnectionFactory() { 
            HostName = _rabbitMqSettings.HostName,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password,
            Port = _rabbitMqSettings.Port,
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "confirmationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendMessage(ConfirmationRequest request)
    {
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
        _channel.BasicPublish(exchange: "", routingKey: "confirmationQueue", basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
