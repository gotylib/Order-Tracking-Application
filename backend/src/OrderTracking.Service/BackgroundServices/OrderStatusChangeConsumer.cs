using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OrderTracking.Domain.Events;
using OrderTracking.Service.Interfaces;

namespace OrderTracking.Service.BackgroundServices;

/// <summary>
/// Фоновый сервис для потребления событий изменения статуса заказа из RabbitMQ.
/// </summary>
public class OrderStatusChangeConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly ILogger<OrderStatusChangeConsumer> _logger;
    private const string ExchangeName = "order_tracking_exchange";
    private const string QueueName = "order_status_changed_queue";
    private const string RoutingKey = "order.status.changed";

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderStatusChangeConsumer"/>.
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов.</param>
    /// <param name="connectionFactory">Фабрика подключений RabbitMQ.</param>
    /// <param name="logger">Логгер.</param>
    public OrderStatusChangeConsumer(
        IServiceProvider serviceProvider,
        IConnectionFactory connectionFactory,
        ILogger<OrderStatusChangeConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        try
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger.LogInformation("RabbitMQ connection established for consumer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection in constructor");
            // Не бросаем исключение, чтобы приложение могло запуститься
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Ждем немного, чтобы RabbitMQ точно был готов
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_connection is not { IsOpen: true })
                {
                    _logger.LogWarning("RabbitMQ connection is not available, retrying in 10 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }
                
                var channel = _channel;
                if (channel == null || channel.IsClosed)
                {
                    _logger.LogInformation("Creating new RabbitMQ channel");
                    channel = _connection.CreateModel();
                }
                
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
                channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(QueueName, ExchangeName, RoutingKey);

                _logger.LogInformation("RabbitMQ queue {QueueName} declared and bound to exchange {ExchangeName}", 
                    QueueName, ExchangeName);
                
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        var orderStatusChangedEvent = JsonSerializer.Deserialize<OrderStatusChangedEvent>(message);
                        if (orderStatusChangedEvent != null)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                            await notificationService.NotifyOrderStatusChangedAsync(orderStatusChangedEvent);
                        }

                        channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation("Событие изменения статуса заказа обработано: {OrderId}",
                            orderStatusChangedEvent?.OrderId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при обработке события изменения статуса заказа");
                        try
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                        catch
                        {
                            // Игнорируем ошибки при Nack
                        }
                    }
                };

                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
                _logger.LogInformation("Consumer для очереди {QueueName} успешно запущен", QueueName);
                
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в Consumer, повторная попытка через 10 секунд");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}
