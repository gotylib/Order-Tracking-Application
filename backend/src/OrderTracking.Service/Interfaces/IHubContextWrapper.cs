using OrderTracking.Domain.Events;

namespace OrderTracking.Service.Interfaces;

/// <summary>
/// Интерфейс для обертки над SignalR Hub Context.
/// </summary>
public interface IHubContextWrapper
{
    /// <summary>
    /// Отправляет событие изменения статуса заказа всем подключенным клиентам.
    /// </summary>
    /// <param name="event">Событие изменения статуса заказа.</param>
    Task SendOrderStatusChangedAsync(OrderStatusChangedEvent @event);
}
