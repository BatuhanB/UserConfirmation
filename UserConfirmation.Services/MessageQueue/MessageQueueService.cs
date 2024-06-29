using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.MessageQueue;
public class MessageQueueService : IMessageQueueService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    public MessageQueueService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
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
