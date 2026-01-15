using OrderTracking.Domain.Events;

namespace OrderTracking.Service.Interfaces;

/// <summary>
/// Интерфейс для публикации сообщений в очередь.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Публикует событие изменения статуса заказа.
    /// </summary>
    /// <param name="event">Событие изменения статуса заказа.</param>
    Task PublishOrderStatusChangedAsync(OrderStatusChangedEvent @event);
}
