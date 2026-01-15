using OrderTracking.Domain.Enums;

namespace OrderTracking.Domain.Events;

/// <summary>
/// Событие изменения статуса заказа.
/// </summary>
public class OrderStatusChangedEvent
{
    /// <summary>
    /// Уникальный идентификатор заказа.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Номер заказа.
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Предыдущий статус заказа.
    /// </summary>
    public OrderStatus PreviousStatus { get; set; }

    /// <summary>
    /// Новый статус заказа.
    /// </summary>
    public OrderStatus NewStatus { get; set; }

    /// <summary>
    /// Дата и время изменения статуса.
    /// </summary>
    public DateTime ChangedAt { get; set; }
}
