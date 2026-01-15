namespace OrderTracking.Domain.DTOs;

/// <summary>
/// DTO для создания нового заказа.
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// Номер заказа.
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Описание заказа.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
