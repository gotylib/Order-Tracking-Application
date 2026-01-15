using OrderTracking.Domain.Events;

namespace OrderTracking.Service.Interfaces;

/// <summary>
/// Интерфейс сервиса уведомлений.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Отправляет уведомление об изменении статуса заказа.
    /// </summary>
    /// <param name="event">Событие изменения статуса заказа.</param>
    Task NotifyOrderStatusChangedAsync(OrderStatusChangedEvent @event);
}
