using Riok.Mapperly.Abstractions;
using OrderTracking.Domain.DTOs;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;

namespace OrderTracking.Service.Mappings;

/// <summary>
/// Маппер для преобразования между сущностями заказов и DTO.
/// Реализация генерируется автоматически Mapperly во время компиляции.
/// </summary>
[Mapper]
public partial class OrderMapper
{
    /// <summary>
    /// Преобразует сущность заказа в DTO.
    /// </summary>
    /// <param name="order">Сущность заказа.</param>
    /// <returns>DTO заказа.</returns>
    public partial OrderDto ToOrderDto(Order order);

    /// <summary>
    /// Преобразует коллекцию сущностей заказов в коллекцию DTO.
    /// </summary>
    /// <param name="orders">Коллекция сущностей заказов.</param>
    /// <returns>Коллекция DTO заказов.</returns>
    public partial IEnumerable<OrderDto> ToOrderDtoList(IEnumerable<Order> orders);

    /// <summary>
    /// Преобразует DTO создания заказа в сущность заказа.
    /// </summary>
    /// <param name="createOrderDto">DTO для создания заказа.</param>
    /// <returns>Сущность заказа.</returns>
    [MapperIgnoreTarget(nameof(Order.Id))]
    [MapperIgnoreTarget(nameof(Order.Status))]
    [MapperIgnoreTarget(nameof(Order.CreatedAt))]
    [MapperIgnoreTarget(nameof(Order.UpdatedAt))]
    public partial Order ToOrder(CreateOrderDto createOrderDto);

    /// <summary>
    /// Создает сущность заказа из DTO с установкой значений по умолчанию.
    /// </summary>
    /// <param name="createOrderDto">DTO для создания заказа.</param>
    /// <returns>Сущность заказа с установленными значениями по умолчанию.</returns>
    public Order ToOrderWithDefaults(CreateOrderDto createOrderDto)
    {
        var order = ToOrder(createOrderDto);
        order.Id = Guid.NewGuid();
        order.Status = OrderStatus.Created;
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        return order;
    }
}
