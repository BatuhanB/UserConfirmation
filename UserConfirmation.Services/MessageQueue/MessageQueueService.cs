﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UserConfirmation.Data.Models;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.MessageQueue;
public class MessageQueueService : IMessageQueueService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IServiceProvider _serviceProvider;
    public MessageQueueService(IOptions<RabbitMqSettings> options, 
        IServiceProvider serviceProvider)
    {
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
        _serviceProvider = serviceProvider;
    }

    public void SendMessage(ConfirmationRequest request)
    {
        var message = $"{request.UserId}:{request.ConfirmationCode}";

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        _channel.BasicPublish(exchange: "", routingKey: "confirmationQueue", basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }

    public Task<string> RecieveMessage()
    {
        var tcs = new TaskCompletionSource<string>();
        var consumer = new EventingBasicConsumer(_channel);

        async void ReceivedHandler(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                message = message.Replace("\"", "").Replace("\\", "");

                var parts = message.Split(':');
                var userId = parts[0];
                var confirmationCode = parts[1];

                // Process the message using IMessageProcessor
                ProcessCode(userId, confirmationCode).Wait();

                tcs.TrySetResult(confirmationCode);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
            finally
            {
                _channel.BasicConsume(queue: "confirmationQueue", autoAck: true, consumer: consumer);
            }
        }

        consumer.Received += ReceivedHandler;
        _channel.BasicConsume(queue: "confirmationQueue", autoAck: true, consumer: consumer);

        return tcs.Task;
    }

    private async Task ProcessCode(string userId,string confirmationCode)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Data.DbContext>();
        var code = new ConfirmationCode(userId, confirmationCode);
        context.ConfirmationCodes.Add(code);
        await context.SaveChangesAsync();
    }
}
