using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using OrderTracking.Domain.Events;
using OrderTracking.Service.Interfaces;

namespace OrderTracking.Service.Services;

/// <summary>
/// Реализация публикатора сообщений через RabbitMQ.
/// </summary>
public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQMessagePublisher> _logger;
    private const string ExchangeName = "order_tracking_exchange";
    private const string QueueName = "order_status_changed_queue";
    private const string RoutingKey = "order.status.changed";

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="RabbitMQMessagePublisher"/>.
    /// </summary>
    /// <param name="connectionFactory">Фабрика подключений RabbitMQ.</param>
    /// <param name="logger">Логгер.</param>
    public RabbitMQMessagePublisher(IConnectionFactory connectionFactory, ILogger<RabbitMQMessagePublisher> logger)
    {
        _logger = logger;
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        // Создаем exchange и queue
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(QueueName, ExchangeName, RoutingKey);

        _logger.LogInformation("RabbitMQ publisher инициализирован. Exchange: {ExchangeName}, Queue: {QueueName}",
            ExchangeName, QueueName);
    }

    /// <inheritdoc/>
    public Task PublishOrderStatusChangedAsync(OrderStatusChangedEvent @event)
    {
        try
        {
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Сообщение о изменении статуса заказа {OrderId} опубликовано в RabbitMQ",
                @event.OrderId);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при публикации сообщения в RabbitMQ для заказа {OrderId}", @event.OrderId);
            throw;
        }
    }

    /// <summary>
    /// Освобождает ресурсы.
    /// </summary>
    public void Dispose()
    {
        _channel.Close();
        _channel.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}
