namespace OrderTracking.Domain.Enums;

/// <summary>
/// Перечисление статусов заказа.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Заказ создан.
    /// </summary>
    Created = 0,

    /// <summary>
    /// Заказ отправлен.
    /// </summary>
    Sent = 1,

    /// <summary>
    /// Заказ доставлен.
    /// </summary>
    Delivered = 2,

    /// <summary>
    /// Заказ отменен.
    /// </summary>
    Cancelled = 3
}
