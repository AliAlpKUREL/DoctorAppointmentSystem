using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using static MongoDB.Driver.WriteConcern;

namespace DoctorAppointmentSystem.Common;

public class RabbitMqService
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;

    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
        string connectionString = configuration.GetConnectionString("RabbitMQ")!;

        var factory = new ConnectionFactory()
        {
            Uri = new Uri(connectionString) 
        }; 
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _queueName = "prescriptions_queue";
        _channel.QueueDeclare(
              queue: _queueName,
              durable: true,
              exclusive: false,
              autoDelete: false,
              arguments: null
          );

    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "",
            routingKey: _queueName,
            basicProperties: null,
            body: body);

        Console.WriteLine(" [x] Sent {0}", message);
    }

    public void ConsumeMessages(Action<string> onMessageReceived)
    {
        if (onMessageReceived == null)
            throw new ArgumentNullException(nameof(onMessageReceived), "Mesaj işleme mantığı boş olamaz!");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
            onMessageReceived(message);
        };

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer
        );

    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
