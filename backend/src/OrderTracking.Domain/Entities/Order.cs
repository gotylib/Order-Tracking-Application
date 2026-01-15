using OrderTracking.Domain.Enums;

namespace OrderTracking.Domain.Entities;

/// <summary>
/// Представляет заказ в системе отслеживания заказов.
/// </summary>
public class Order
{
    /// <summary>
    /// Уникальный идентификатор заказа.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Номер заказа.
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Описание заказа.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Текущий статус заказа.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Дата и время создания заказа.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления заказа.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
