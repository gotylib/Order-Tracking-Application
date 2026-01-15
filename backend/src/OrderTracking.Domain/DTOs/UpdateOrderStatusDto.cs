using OrderTracking.Domain.Enums;

namespace OrderTracking.Domain.DTOs;

/// <summary>
/// DTO для обновления статуса заказа.
/// </summary>
public class UpdateOrderStatusDto
{
    /// <summary>
    /// Новый статус заказа.
    /// </summary>
    public OrderStatus Status { get; set; }
}
